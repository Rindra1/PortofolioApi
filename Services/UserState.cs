namespace PortofolioApi.Services;

// /Services/UserState.cs
public class UserState
{
    public event Action OnChange;

    private string _role;
    public string Role
    {
        get => _role;
        set
        {
            _role = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

