namespace AutoPartsStore.Api.Contracts.Auth;

public record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string? PhoneNumber);

public record LoginRequest(
    string Email,
    string Password);

public record AuthResponse(
    string Token,
    Guid UserId,
    string Name,
    string Email,
    string Role);
