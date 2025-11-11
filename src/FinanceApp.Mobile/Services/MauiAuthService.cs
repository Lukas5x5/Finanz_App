using FinanceApp.Web.Services;
using Supabase.Gotrue;

namespace FinanceApp.Mobile.Services;

public class MauiAuthService : ISupabaseAuthService
{
    private readonly Supabase.Client _client;
    private readonly AppState _appState;

    public MauiAuthService(Supabase.Client client, AppState appState)
    {
        _client = client;
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
            SecureStorage.Default.Remove("supabase.auth.token");
            SecureStorage.Default.Remove("supabase.auth.refresh_token");
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
        try
        {
            var token = await SecureStorage.Default.GetAsync("supabase.auth.token");
            return !string.IsNullOrEmpty(token);
        }
        catch
        {
            return false;
        }
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

            // Try to restore session from SecureStorage
            var token = await SecureStorage.Default.GetAsync("supabase.auth.token");
            var refreshToken = await SecureStorage.Default.GetAsync("supabase.auth.refresh_token");

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
            return await SecureStorage.Default.GetAsync("supabase.auth.token");
        }
        catch
        {
            return null;
        }
    }

    private async Task SaveSessionAsync(string? accessToken, string? refreshToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await SecureStorage.Default.SetAsync("supabase.auth.token", accessToken);
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await SecureStorage.Default.SetAsync("supabase.auth.refresh_token", refreshToken);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving session: {ex.Message}");
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
