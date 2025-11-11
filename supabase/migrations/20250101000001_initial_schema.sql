-- Initial Schema for Finance App
-- Author: Finance App Team
-- Date: 2025-01-01

-- Enable necessary extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================================
-- ORGANIZATIONS TABLE
-- ============================================================================
CREATE TABLE IF NOT EXISTS public.organizations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name TEXT NOT NULL,
    type TEXT NOT NULL CHECK (type IN ('Personal', 'Business')),
    owner_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
);

CREATE INDEX idx_organizations_owner ON public.organizations(owner_id);
CREATE INDEX idx_organizations_type ON public.organizations(type);

COMMENT ON TABLE public.organizations IS 'Organizations (Personal or Business)';
COMMENT ON COLUMN public.organizations.type IS 'Organization type: Personal or Business';

-- ============================================================================
-- MEMBERSHIPS TABLE
-- ============================================================================
CREATE TABLE IF NOT EXISTS public.memberships (
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    organization_id UUID NOT NULL REFERENCES public.organizations(id) ON DELETE CASCADE,
    role TEXT NOT NULL CHECK (role IN ('Owner', 'Admin', 'Member')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    PRIMARY KEY (user_id, organization_id)
);

CREATE INDEX idx_memberships_user ON public.memberships(user_id);
CREATE INDEX idx_memberships_org ON public.memberships(organization_id);
CREATE INDEX idx_memberships_role ON public.memberships(role);

COMMENT ON TABLE public.memberships IS 'User memberships in organizations';
COMMENT ON COLUMN public.memberships.role IS 'User role: Owner, Admin, or Member';

-- ============================================================================
-- COST_ITEMS TABLE
-- ============================================================================
CREATE TABLE IF NOT EXISTS public.cost_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    organization_id UUID NOT NULL REFERENCES public.organizations(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    category TEXT,
    amount NUMERIC(12, 2) NOT NULL CHECK (amount > 0),
    currency CHAR(3) NOT NULL DEFAULT 'EUR',
    cycle TEXT NOT NULL CHECK (cycle IN ('Monthly', 'Yearly')),
    has_binding BOOLEAN NOT NULL DEFAULT FALSE,
    binding_ends_at DATE,
    payment_method TEXT,
    notes TEXT,
    tags TEXT[] DEFAULT '{}',
    created_by UUID NOT NULL REFERENCES auth.users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    CONSTRAINT binding_date_required CHECK (
        (has_binding = FALSE AND binding_ends_at IS NULL) OR
        (has_binding = TRUE AND binding_ends_at IS NOT NULL)
    )
);

CREATE INDEX idx_cost_items_org ON public.cost_items(organization_id);
CREATE INDEX idx_cost_items_cycle ON public.cost_items(organization_id, cycle);
CREATE INDEX idx_cost_items_binding ON public.cost_items(organization_id, binding_ends_at)
    WHERE has_binding = TRUE;
CREATE INDEX idx_cost_items_category ON public.cost_items(organization_id, category);
CREATE INDEX idx_cost_items_tags ON public.cost_items USING GIN(tags);

COMMENT ON TABLE public.cost_items IS 'Recurring cost items (subscriptions, contracts)';
COMMENT ON COLUMN public.cost_items.cycle IS 'Billing cycle: Monthly or Yearly';
COMMENT ON COLUMN public.cost_items.has_binding IS 'Whether this cost has a contract binding';
COMMENT ON COLUMN public.cost_items.binding_ends_at IS 'Date when contract binding ends';

-- ============================================================================
-- INVOICES TABLE
-- ============================================================================
CREATE TABLE IF NOT EXISTS public.invoices (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    organization_id UUID NOT NULL REFERENCES public.organizations(id) ON DELETE CASCADE,
    vendor TEXT NOT NULL,
    amount NUMERIC(12, 2) NOT NULL CHECK (amount > 0),
    currency CHAR(3) NOT NULL DEFAULT 'EUR',
    due_at DATE NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('Open', 'Paid')) DEFAULT 'Open',
    category TEXT,
    notes TEXT,
    created_by UUID NOT NULL REFERENCES auth.users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
);

CREATE INDEX idx_invoices_org ON public.invoices(organization_id);
CREATE INDEX idx_invoices_due ON public.invoices(organization_id, due_at);
CREATE INDEX idx_invoices_status ON public.invoices(organization_id, status);
CREATE INDEX idx_invoices_category ON public.invoices(organization_id, category);

COMMENT ON TABLE public.invoices IS 'Open and paid invoices';
COMMENT ON COLUMN public.invoices.status IS 'Invoice status: Open or Paid';
COMMENT ON COLUMN public.invoices.due_at IS 'Invoice due date';

-- ============================================================================
-- ATTACHMENTS TABLE
-- ============================================================================
CREATE TABLE IF NOT EXISTS public.attachments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    invoice_id UUID NOT NULL REFERENCES public.invoices(id) ON DELETE CASCADE,
    object_path TEXT NOT NULL,
    content_type TEXT,
    size_bytes BIGINT,
    created_by UUID NOT NULL REFERENCES auth.users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
);

CREATE INDEX idx_attachments_invoice ON public.attachments(invoice_id);

COMMENT ON TABLE public.attachments IS 'File attachments for invoices (stored in Supabase Storage)';
COMMENT ON COLUMN public.attachments.object_path IS 'Path to file in Supabase Storage';

-- ============================================================================
-- TRIGGER: Update updated_at timestamp
-- ============================================================================
CREATE OR REPLACE FUNCTION public.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_organizations_updated_at BEFORE UPDATE ON public.organizations
    FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER update_cost_items_updated_at BEFORE UPDATE ON public.cost_items
    FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER update_invoices_updated_at BEFORE UPDATE ON public.invoices
    FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

-- ============================================================================
-- TRIGGER: Auto-create membership for organization owner
-- ============================================================================
CREATE OR REPLACE FUNCTION public.create_owner_membership()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.memberships (user_id, organization_id, role)
    VALUES (NEW.owner_id, NEW.id, 'Owner');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER create_owner_membership_trigger AFTER INSERT ON public.organizations
    FOR EACH ROW EXECUTE FUNCTION public.create_owner_membership();
