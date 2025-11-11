using Blazored.LocalStorage;
using Supabase.Gotrue;
using Gotrue = Supabase.Gotrue;

namespace FinanceApp.Web.Services;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly Supabase.Client _client;
    private readonly ILocalStorageService _localStorage;
    private readonly AppState _appState;

    public SupabaseAuthService(
        Supabase.Client client,
        ILocalStorageService localStorage,
        AppState appState)
    {
        _client = client;
        _localStorage = localStorage;
        _appState = appState;
    }

    public async Task<bool> SignUpAsync(string email, string password)
    {
        try
        {
            var response = await _client.Auth.SignUp(email, password);

            if (response?.User != null)
            {
                await SaveSessionAsync(response.AccessToken, response.RefreshToken);
                await UpdateAppStateAsync(response.User);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Sign up error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        try
        {
            var response = await _client.Auth.SignIn(email, password);

            if (response?.User != null)
            {
                await SaveSessionAsync(response.AccessToken, response.RefreshToken);
                await UpdateAppStateAsync(response.User);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Sign in error: {ex.Message}");
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        try
        {
            await _client.Auth.SignOut();
            await _localStorage.RemoveItemAsync("supabase.auth.token");
            await _localStorage.RemoveItemAsync("supabase.auth.refresh_token");
            _appState.CurrentUser = null;
            _appState.CurrentOrganization = null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Sign out error: {ex.Message}");
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync("supabase.auth.token");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            var user = _client.Auth.CurrentUser;
            if (user != null)
            {
                return new User
                {
                    Id = Guid.Parse(user.Id),
                    Email = user.Email ?? string.Empty
                };
            }

            // Try to restore session from local storage
            var token = await _localStorage.GetItemAsStringAsync("supabase.auth.token");
            var refreshToken = await _localStorage.GetItemAsStringAsync("supabase.auth.refresh_token");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
            {
                await _client.Auth.SetSession(token, refreshToken);
                user = _client.Auth.CurrentUser;

                if (user != null)
                {
                    return new User
                    {
                        Id = Guid.Parse(user.Id),
                        Email = user.Email ?? string.Empty
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Get current user error: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            return await _localStorage.GetItemAsStringAsync("supabase.auth.token");
        }
        catch
        {
            return null;
        }
    }

    private async Task SaveSessionAsync(string? accessToken, string? refreshToken)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            await _localStorage.SetItemAsStringAsync("supabase.auth.token", accessToken);
        }

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _localStorage.SetItemAsStringAsync("supabase.auth.refresh_token", refreshToken);
        }
    }

    private async Task UpdateAppStateAsync(Gotrue.User user)
    {
        _appState.CurrentUser = new User
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email ?? string.Empty
        };

        await Task.CompletedTask;
    }
}
