using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Api.Auth;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    UserRole? Role { get; }

    bool IsAuthenticated { get; }

    bool IsAdmin { get; }

    bool IsAdminOrCreator { get; }
}
