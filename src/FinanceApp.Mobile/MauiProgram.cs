using Microsoft.Extensions.Logging;
using FinanceApp.Web.Services;

namespace FinanceApp.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Supabase Configuration
        var supabaseUrl = "https://nqbkogbynurtjtppajic.supabase.co";
        var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5xYmtvZ2J5bnVydGp0cHBhamljIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjI4MTcxNDQsImV4cCI6MjA3ODM5MzE0NH0.GFYWYprO4HaA0vCzF4aHZNOvb5iLV0kXPnPGiIeotW8";

        builder.Services.AddScoped(sp => new Supabase.Client(supabaseUrl, supabaseKey, new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }));

        // Application Services
        builder.Services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();
        builder.Services.AddScoped<IOrganizationService, OrganizationService>();
        builder.Services.AddScoped<ICostRepository, CostRepository>();
        builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        builder.Services.AddScoped<IStorageService, StorageService>();
        builder.Services.AddScoped<ISummaryService, SummaryService>();
        builder.Services.AddScoped<AppState>();

        // Platform-specific services
        builder.Services.AddScoped<ICameraService, CameraService>();

        return builder.Build();
    }
}
