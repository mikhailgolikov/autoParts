using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Api.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    string? PhoneNumber,
    UserRole Role,
    DateTime CreatedAt);

public record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    string? PhoneNumber,
    UserRole Role = UserRole.Client);

public record UpdateUserRequest(
    string Name,
    string? PhoneNumber);

public record UpdateProfileRequest(
    string Name,
    string Email,
    string? PhoneNumber);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

public record ChangeUserRoleRequest(UserRole Role);
