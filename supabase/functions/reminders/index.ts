// Supabase Edge Function: Reminders
// Sends email reminders for upcoming invoice due dates and contract bindings
// Scheduled to run daily via GitHub Actions or Supabase Schedule

import { serve } from "https://deno.land/std@0.168.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.38.4";

interface Invoice {
  id: string;
  vendor: string;
  amount: number;
  currency: string;
  due_at: string;
  organization_id: string;
}

interface CostItem {
  id: string;
  name: string;
  binding_ends_at: string;
  organization_id: string;
}

interface OrgUser {
  user_id: string;
  organization_id: string;
  email: string;
}

serve(async (req) => {
  try {
    // Initialize Supabase client with service role
    const supabaseUrl = Deno.env.get("SUPABASE_URL")!;
    const supabaseServiceKey = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY")!;
    const supabase = createClient(supabaseUrl, supabaseServiceKey);

    const today = new Date();
    const in7Days = new Date(today);
    in7Days.setDate(today.getDate() + 7);
    const in3Days = new Date(today);
    in3Days.setDate(today.getDate() + 3);
    const in1Day = new Date(today);
    in1Day.setDate(today.getDate() + 1);
    const in30Days = new Date(today);
    in30Days.setDate(today.getDate() + 30);

    // Format dates for SQL
    const formatDate = (d: Date) => d.toISOString().split("T")[0];

    // ========================================================================
    // Find invoices due soon (7, 3, 1 days)
    // ========================================================================
    const { data: invoicesDue, error: invoicesError } = await supabase
      .from("invoices")
      .select("id, vendor, amount, currency, due_at, organization_id")
      .eq("status", "Open")
      .in("due_at", [
        formatDate(in7Days),
        formatDate(in3Days),
        formatDate(in1Day),
      ]);

    if (invoicesError) {
      console.error("Error fetching invoices:", invoicesError);
    }

    // ========================================================================
    // Find cost items with bindings ending soon (30, 7 days)
    // ========================================================================
    const { data: bindingsEnding, error: bindingsError } = await supabase
      .from("cost_items")
      .select("id, name, binding_ends_at, organization_id")
      .eq("has_binding", true)
      .in("binding_ends_at", [formatDate(in30Days), formatDate(in7Days)]);

    if (bindingsError) {
      console.error("Error fetching bindings:", bindingsError);
    }

    // ========================================================================
    // Group by organization and send emails
    // ========================================================================
    const organizations = new Set<string>();

    invoicesDue?.forEach((inv: Invoice) =>
      organizations.add(inv.organization_id)
    );
    bindingsEnding?.forEach((cost: CostItem) =>
      organizations.add(cost.organization_id)
    );

    const reminders = [];

    for (const orgId of organizations) {
      // Get users in this organization
      const { data: members } = await supabase
        .from("memberships")
        .select(
          `
          user_id,
          organization_id
        `
        )
        .eq("organization_id", orgId);

      if (!members || members.length === 0) continue;

      // Get user emails
      const userIds = members.map((m: { user_id: string }) => m.user_id);
      const { data: users } = await supabase.auth.admin.listUsers();
      const orgUsers =
        users?.users.filter((u: { id: string }) => userIds.includes(u.id)) ||
        [];

      // Prepare reminder data for this org
      const orgInvoices = invoicesDue?.filter(
        (inv: Invoice) => inv.organization_id === orgId
      );
      const orgBindings = bindingsEnding?.filter(
        (cost: CostItem) => cost.organization_id === orgId
      );

      // Send email to each member
      for (const user of orgUsers) {
        const reminderData = {
          to: user.email,
          subject: "Finance App: Erinnerungen und Fälligkeiten",
          invoices: orgInvoices,
          bindings: orgBindings,
        };

        // Log reminder (in production, send actual email via SMTP or service)
        console.log("Sending reminder:", reminderData);
        reminders.push(reminderData);

        // Optional: Send email via Supabase SMTP or external service
        // await sendEmail(reminderData);
      }

      // Log to audit table (optional)
      await supabase.from("reminder_logs").insert({
        organization_id: orgId,
        reminder_type: "daily_check",
        invoices_count: orgInvoices?.length || 0,
        bindings_count: orgBindings?.length || 0,
        sent_at: new Date().toISOString(),
      });
    }

    return new Response(
      JSON.stringify({
        success: true,
        reminders_sent: reminders.length,
        organizations_processed: organizations.size,
        invoices_checked: invoicesDue?.length || 0,
        bindings_checked: bindingsEnding?.length || 0,
        timestamp: new Date().toISOString(),
      }),
      {
        headers: { "Content-Type": "application/json" },
        status: 200,
      }
    );
  } catch (error) {
    console.error("Error in reminders function:", error);
    return new Response(
      JSON.stringify({
        success: false,
        error: error.message,
      }),
      {
        headers: { "Content-Type": "application/json" },
        status: 500,
      }
    );
  }
});
