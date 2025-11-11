using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FinanceApp.Web;
using FinanceApp.Web.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// Supabase Configuration
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL not configured");
var supabaseKey = builder.Configuration["Supabase:AnonKey"] ?? throw new InvalidOperationException("Supabase Anon Key not configured");

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

await builder.Build().RunAsync();
