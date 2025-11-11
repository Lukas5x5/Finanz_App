-- Views and RPC Functions
-- Author: Finance App Team
-- Date: 2025-01-01

-- ============================================================================
-- VIEW: Monthly costs summary
-- ============================================================================
CREATE OR REPLACE VIEW public.v_monthly_costs AS
SELECT
    organization_id,
    SUM(
        CASE
            WHEN cycle = 'Monthly' THEN amount
            WHEN cycle = 'Yearly' THEN amount / 12
            ELSE 0
        END
    ) AS total_monthly,
    SUM(
        CASE
            WHEN cycle = 'Monthly' THEN amount * 12
            WHEN cycle = 'Yearly' THEN amount
            ELSE 0
        END
    ) AS total_yearly,
    COUNT(*) AS item_count,
    COUNT(*) FILTER (WHERE has_binding = TRUE) AS binding_count
FROM public.cost_items
GROUP BY organization_id;

COMMENT ON VIEW public.v_monthly_costs IS 'Monthly and yearly cost summaries per organization';

-- ============================================================================
-- RPC: Get month summary for organization
-- ============================================================================
CREATE OR REPLACE FUNCTION public.rpc_get_month_summary(org_id UUID)
RETURNS JSON AS $$
DECLARE
    result JSON;
    total_month NUMERIC(12, 2);
    total_year NUMERIC(12, 2);
    open_invoices NUMERIC(12, 2);
    next_30_days NUMERIC(12, 2);
    upcoming_bindings JSON;
BEGIN
    -- Check if user is member
    IF NOT public.is_member(org_id) THEN
        RAISE EXCEPTION 'Access denied';
    END IF;

    -- Calculate totals from cost items
    SELECT
        COALESCE(SUM(
            CASE
                WHEN cycle = 'Monthly' THEN amount
                WHEN cycle = 'Yearly' THEN amount / 12
                ELSE 0
            END
        ), 0),
        COALESCE(SUM(
            CASE
                WHEN cycle = 'Monthly' THEN amount * 12
                WHEN cycle = 'Yearly' THEN amount
                ELSE 0
            END
        ), 0)
    INTO total_month, total_year
    FROM public.cost_items
    WHERE organization_id = org_id;

    -- Calculate open invoices total
    SELECT COALESCE(SUM(amount), 0)
    INTO open_invoices
    FROM public.invoices
    WHERE organization_id = org_id
    AND status = 'Open';

    -- Calculate next 30 days cash out
    SELECT COALESCE(SUM(amount), 0)
    INTO next_30_days
    FROM public.invoices
    WHERE organization_id = org_id
    AND status = 'Open'
    AND due_at BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '30 days';

    -- Get upcoming bindings
    SELECT json_agg(
        json_build_object(
            'cost_item_id', id,
            'name', name,
            'binding_ends_at', binding_ends_at,
            'days_until_end', binding_ends_at - CURRENT_DATE
        ) ORDER BY binding_ends_at
    )
    INTO upcoming_bindings
    FROM public.cost_items
    WHERE organization_id = org_id
    AND has_binding = TRUE
    AND binding_ends_at >= CURRENT_DATE
    AND binding_ends_at <= CURRENT_DATE + INTERVAL '60 days';

    -- Build result
    result := json_build_object(
        'total_monthly', total_month,
        'total_yearly', total_year,
        'open_invoices', open_invoices,
        'next_30_days_cash_out', next_30_days,
        'upcoming_bindings', COALESCE(upcoming_bindings, '[]'::json)
    );

    RETURN result;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.rpc_get_month_summary IS 'Returns comprehensive financial summary for organization';

-- ============================================================================
-- RPC: Get invoices due within days
-- ============================================================================
CREATE OR REPLACE FUNCTION public.rpc_get_invoices_due(
    org_id UUID,
    days_ahead INT DEFAULT 30
)
RETURNS TABLE (
    id UUID,
    vendor TEXT,
    amount NUMERIC(12, 2),
    currency CHAR(3),
    due_at DATE,
    status TEXT,
    category TEXT,
    notes TEXT,
    days_until_due INT
) AS $$
BEGIN
    -- Check if user is member
    IF NOT public.is_member(org_id) THEN
        RAISE EXCEPTION 'Access denied';
    END IF;

    RETURN QUERY
    SELECT
        i.id,
        i.vendor,
        i.amount,
        i.currency,
        i.due_at,
        i.status,
        i.category,
        i.notes,
        (i.due_at - CURRENT_DATE) AS days_until_due
    FROM public.invoices i
    WHERE i.organization_id = org_id
    AND i.status = 'Open'
    AND i.due_at BETWEEN CURRENT_DATE AND CURRENT_DATE + days_ahead
    ORDER BY i.due_at;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.rpc_get_invoices_due IS 'Returns open invoices due within specified days';

-- ============================================================================
-- RPC: Export costs to CSV format
-- ============================================================================
CREATE OR REPLACE FUNCTION public.rpc_export_costs_csv(org_id UUID)
RETURNS TEXT AS $$
DECLARE
    csv_output TEXT;
BEGIN
    -- Check if user is member
    IF NOT public.is_member(org_id) THEN
        RAISE EXCEPTION 'Access denied';
    END IF;

    -- Build CSV
    SELECT string_agg(
        format('%s,%s,%s,%s,%s,%s,%s,%s',
            name,
            COALESCE(category, ''),
            amount,
            currency,
            cycle,
            CASE WHEN has_binding THEN 'Yes' ELSE 'No' END,
            COALESCE(binding_ends_at::TEXT, ''),
            COALESCE(payment_method, '')
        ),
        E'\n'
    )
    INTO csv_output
    FROM (
        SELECT * FROM public.cost_items
        WHERE organization_id = org_id
        ORDER BY name
    ) AS costs;

    -- Add header
    csv_output := 'Name,Category,Amount,Currency,Cycle,Has Binding,Binding Ends,Payment Method' || E'\n' || COALESCE(csv_output, '');

    RETURN csv_output;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.rpc_export_costs_csv IS 'Exports cost items as CSV text';

-- ============================================================================
-- RPC: Export invoices to CSV format
-- ============================================================================
CREATE OR REPLACE FUNCTION public.rpc_export_invoices_csv(org_id UUID)
RETURNS TEXT AS $$
DECLARE
    csv_output TEXT;
BEGIN
    -- Check if user is member
    IF NOT public.is_member(org_id) THEN
        RAISE EXCEPTION 'Access denied';
    END IF;

    -- Build CSV
    SELECT string_agg(
        format('%s,%s,%s,%s,%s,%s',
            vendor,
            amount,
            currency,
            due_at,
            status,
            COALESCE(category, '')
        ),
        E'\n'
    )
    INTO csv_output
    FROM (
        SELECT * FROM public.invoices
        WHERE organization_id = org_id
        ORDER BY due_at DESC
    ) AS invoices;

    -- Add header
    csv_output := 'Vendor,Amount,Currency,Due Date,Status,Category' || E'\n' || COALESCE(csv_output, '');

    RETURN csv_output;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.rpc_export_invoices_csv IS 'Exports invoices as CSV text';

-- ============================================================================
-- RPC: Auto-create personal organization on signup
-- ============================================================================
CREATE OR REPLACE FUNCTION public.rpc_create_personal_organization()
RETURNS UUID AS $$
DECLARE
    new_org_id UUID;
    user_email TEXT;
BEGIN
    -- Get user email
    SELECT email INTO user_email FROM auth.users WHERE id = auth.uid();

    -- Check if user already has a personal organization
    SELECT id INTO new_org_id
    FROM public.organizations
    WHERE owner_id = auth.uid()
    AND type = 'Personal'
    LIMIT 1;

    -- Create if doesn't exist
    IF new_org_id IS NULL THEN
        INSERT INTO public.organizations (name, type, owner_id)
        VALUES (
            COALESCE(split_part(user_email, '@', 1), 'Personal') || ' - Privat',
            'Personal',
            auth.uid()
        )
        RETURNING id INTO new_org_id;
    END IF;

    RETURN new_org_id;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.rpc_create_personal_organization IS 'Creates a personal organization for current user';

-- ============================================================================
-- RPC: Get dashboard stats
-- ============================================================================
CREATE OR REPLACE FUNCTION public.rpc_get_dashboard_stats(org_id UUID)
RETURNS JSON AS $$
DECLARE
    result JSON;
BEGIN
    -- Check if user is member
    IF NOT public.is_member(org_id) THEN
        RAISE EXCEPTION 'Access denied';
    END IF;

    SELECT json_build_object(
        'cost_items_count', (
            SELECT COUNT(*) FROM public.cost_items WHERE organization_id = org_id
        ),
        'open_invoices_count', (
            SELECT COUNT(*) FROM public.invoices
            WHERE organization_id = org_id AND status = 'Open'
        ),
        'paid_invoices_count', (
            SELECT COUNT(*) FROM public.invoices
            WHERE organization_id = org_id AND status = 'Paid'
        ),
        'bindings_expiring_30d', (
            SELECT COUNT(*) FROM public.cost_items
            WHERE organization_id = org_id
            AND has_binding = TRUE
            AND binding_ends_at BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '30 days'
        ),
        'invoices_due_7d', (
            SELECT COUNT(*) FROM public.invoices
            WHERE organization_id = org_id
            AND status = 'Open'
            AND due_at BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '7 days'
        )
    ) INTO result;

    RETURN result;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.rpc_get_dashboard_stats IS 'Returns dashboard statistics for organization';
