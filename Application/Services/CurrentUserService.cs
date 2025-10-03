namespace PortofolioApi.Application.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId =>
        int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value, out var id)
        ? id : (int?)null;
}
