-- ============================================================================
-- FIX: Membership creation trigger for new organizations
-- ============================================================================
-- Problem: When creating a new organization, the trigger tries to insert
-- a membership, but RLS policies block it because the user is not yet
-- an owner/admin of the organization (chicken-egg problem).
--
-- Solution: Make the trigger function run with SECURITY DEFINER,
-- which bypasses RLS policies for the function execution.
-- ============================================================================

-- Drop existing trigger and function
DROP TRIGGER IF EXISTS create_owner_membership_trigger ON public.organizations;
DROP FUNCTION IF EXISTS public.create_owner_membership();

-- Recreate function with SECURITY DEFINER
CREATE OR REPLACE FUNCTION public.create_owner_membership()
RETURNS TRIGGER
SECURITY DEFINER  -- This allows the function to bypass RLS policies
SET search_path = public
AS $$
BEGIN
    INSERT INTO public.memberships (user_id, organization_id, role)
    VALUES (NEW.owner_id, NEW.id, 'Owner');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Recreate trigger
CREATE TRIGGER create_owner_membership_trigger
    AFTER INSERT ON public.organizations
    FOR EACH ROW
    EXECUTE FUNCTION public.create_owner_membership();

COMMENT ON FUNCTION public.create_owner_membership() IS
'Automatically creates an Owner membership when a new organization is created. Uses SECURITY DEFINER to bypass RLS policies.';
