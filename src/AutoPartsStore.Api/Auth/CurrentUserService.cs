using System.Security.Claims;
using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Api.Auth;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(value, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated => UserId.HasValue;

    public bool IsAdmin => Role == UserRole.Admin;

    public bool IsAdminOrCreator => Role is UserRole.Admin or UserRole.Creator;
}
