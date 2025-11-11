namespace FinanceApp.Web.Services;

public interface ISupabaseAuthService
{
    Task<bool> SignUpAsync(string email, string password);
    Task<bool> SignInAsync(string email, string password);
    Task SignOutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<User?> GetCurrentUserAsync();
    Task<string?> GetAccessTokenAsync();
}
