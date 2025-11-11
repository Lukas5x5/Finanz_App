-- Row Level Security Policies
-- Author: Finance App Team
-- Date: 2025-01-01

-- ============================================================================
-- HELPER FUNCTION: Check if user is member of organization
-- ============================================================================
CREATE OR REPLACE FUNCTION public.is_member(org_id UUID)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM public.memberships
        WHERE organization_id = org_id
        AND user_id = auth.uid()
    );
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.is_member IS 'Checks if current user is member of given organization';

-- ============================================================================
-- HELPER FUNCTION: Check if user is owner or admin
-- ============================================================================
CREATE OR REPLACE FUNCTION public.is_owner_or_admin(org_id UUID)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM public.memberships
        WHERE organization_id = org_id
        AND user_id = auth.uid()
        AND role IN ('Owner', 'Admin')
    );
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.is_owner_or_admin IS 'Checks if current user is owner or admin of given organization';

-- ============================================================================
-- HELPER FUNCTION: Check if user is organization owner
-- ============================================================================
CREATE OR REPLACE FUNCTION public.is_owner(org_id UUID)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM public.organizations
        WHERE id = org_id
        AND owner_id = auth.uid()
    );
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

COMMENT ON FUNCTION public.is_owner IS 'Checks if current user is owner of given organization';

-- ============================================================================
-- RLS: ORGANIZATIONS
-- ============================================================================
ALTER TABLE public.organizations ENABLE ROW LEVEL SECURITY;

-- Users can view organizations they own or are members of
CREATE POLICY "Users can view their organizations"
    ON public.organizations
    FOR SELECT
    USING (
        owner_id = auth.uid() OR
        public.is_member(id)
    );

-- Users can create organizations
CREATE POLICY "Users can create organizations"
    ON public.organizations
    FOR INSERT
    WITH CHECK (owner_id = auth.uid());

-- Only owners can update their organizations
CREATE POLICY "Owners can update their organizations"
    ON public.organizations
    FOR UPDATE
    USING (owner_id = auth.uid())
    WITH CHECK (owner_id = auth.uid());

-- Only owners can delete their organizations
CREATE POLICY "Owners can delete their organizations"
    ON public.organizations
    FOR DELETE
    USING (owner_id = auth.uid());

-- ============================================================================
-- RLS: MEMBERSHIPS
-- ============================================================================
ALTER TABLE public.memberships ENABLE ROW LEVEL SECURITY;

-- Users can view memberships of organizations they belong to
CREATE POLICY "Users can view memberships of their organizations"
    ON public.memberships
    FOR SELECT
    USING (
        user_id = auth.uid() OR
        public.is_member(organization_id)
    );

-- Only owners and admins can create memberships
CREATE POLICY "Owners and admins can create memberships"
    ON public.memberships
    FOR INSERT
    WITH CHECK (public.is_owner_or_admin(organization_id));

-- Only owners and admins can update memberships
CREATE POLICY "Owners and admins can update memberships"
    ON public.memberships
    FOR UPDATE
    USING (public.is_owner_or_admin(organization_id))
    WITH CHECK (public.is_owner_or_admin(organization_id));

-- Only owners and admins can delete memberships
CREATE POLICY "Owners and admins can delete memberships"
    ON public.memberships
    FOR DELETE
    USING (public.is_owner_or_admin(organization_id));

-- ============================================================================
-- RLS: COST_ITEMS
-- ============================================================================
ALTER TABLE public.cost_items ENABLE ROW LEVEL SECURITY;

-- Members can view cost items of their organizations
CREATE POLICY "Members can view cost items"
    ON public.cost_items
    FOR SELECT
    USING (public.is_member(organization_id));

-- Members can create cost items
CREATE POLICY "Members can create cost items"
    ON public.cost_items
    FOR INSERT
    WITH CHECK (
        public.is_member(organization_id) AND
        created_by = auth.uid()
    );

-- Members can update cost items
CREATE POLICY "Members can update cost items"
    ON public.cost_items
    FOR UPDATE
    USING (public.is_member(organization_id))
    WITH CHECK (public.is_member(organization_id));

-- Members can delete cost items
CREATE POLICY "Members can delete cost items"
    ON public.cost_items
    FOR DELETE
    USING (public.is_member(organization_id));

-- ============================================================================
-- RLS: INVOICES
-- ============================================================================
ALTER TABLE public.invoices ENABLE ROW LEVEL SECURITY;

-- Members can view invoices of their organizations
CREATE POLICY "Members can view invoices"
    ON public.invoices
    FOR SELECT
    USING (public.is_member(organization_id));

-- Members can create invoices
CREATE POLICY "Members can create invoices"
    ON public.invoices
    FOR INSERT
    WITH CHECK (
        public.is_member(organization_id) AND
        created_by = auth.uid()
    );

-- Members can update invoices
CREATE POLICY "Members can update invoices"
    ON public.invoices
    FOR UPDATE
    USING (public.is_member(organization_id))
    WITH CHECK (public.is_member(organization_id));

-- Members can delete invoices
CREATE POLICY "Members can delete invoices"
    ON public.invoices
    FOR DELETE
    USING (public.is_member(organization_id));

-- ============================================================================
-- RLS: ATTACHMENTS
-- ============================================================================
ALTER TABLE public.attachments ENABLE ROW LEVEL SECURITY;

-- Members can view attachments of invoices in their organizations
CREATE POLICY "Members can view attachments"
    ON public.attachments
    FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM public.invoices
            WHERE invoices.id = attachments.invoice_id
            AND public.is_member(invoices.organization_id)
        )
    );

-- Members can create attachments
CREATE POLICY "Members can create attachments"
    ON public.attachments
    FOR INSERT
    WITH CHECK (
        EXISTS (
            SELECT 1 FROM public.invoices
            WHERE invoices.id = attachments.invoice_id
            AND public.is_member(invoices.organization_id)
        ) AND
        created_by = auth.uid()
    );

-- Members can delete attachments
CREATE POLICY "Members can delete attachments"
    ON public.attachments
    FOR DELETE
    USING (
        EXISTS (
            SELECT 1 FROM public.invoices
            WHERE invoices.id = attachments.invoice_id
            AND public.is_member(invoices.organization_id)
        )
    );

-- ============================================================================
-- Storage Bucket: invoices
-- ============================================================================
INSERT INTO storage.buckets (id, name, public)
VALUES ('invoices', 'invoices', false)
ON CONFLICT (id) DO NOTHING;

-- Storage policies for invoices bucket
CREATE POLICY "Authenticated users can upload invoices"
    ON storage.objects FOR INSERT
    WITH CHECK (
        bucket_id = 'invoices' AND
        auth.role() = 'authenticated'
    );

CREATE POLICY "Users can view their organization's invoices"
    ON storage.objects FOR SELECT
    USING (
        bucket_id = 'invoices' AND
        auth.role() = 'authenticated' AND
        EXISTS (
            SELECT 1
            FROM public.attachments a
            JOIN public.invoices i ON i.id = a.invoice_id
            WHERE a.object_path = storage.objects.name
            AND public.is_member(i.organization_id)
        )
    );

CREATE POLICY "Users can delete their organization's invoices"
    ON storage.objects FOR DELETE
    USING (
        bucket_id = 'invoices' AND
        auth.role() = 'authenticated' AND
        EXISTS (
            SELECT 1
            FROM public.attachments a
            JOIN public.invoices i ON i.id = a.invoice_id
            WHERE a.object_path = storage.objects.name
            AND public.is_member(i.organization_id)
        )
    );
