using FinanceApp.Shared.Models;

namespace FinanceApp.Web.Services;

/// <summary>
/// Global application state
/// </summary>
public class AppState
{
    private Organization? _currentOrganization;
    private User? _currentUser;
    private string _theme = "light";

    public Organization? CurrentOrganization
    {
        get => _currentOrganization;
        set
        {
            _currentOrganization = value;
            NotifyStateChanged();
        }
    }

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyStateChanged();
        }
    }

    public string Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            NotifyStateChanged();
        }
    }

    public bool IsAuthenticated => _currentUser != null;
    public bool HasOrganization => _currentOrganization != null;

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
