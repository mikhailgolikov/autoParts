using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Auth;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Domain.Enums;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class AuthService(AppDbContext dbContext, JwtTokenService jwtTokenService)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user) =>
        new(
            jwtTokenService.GenerateToken(user),
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString());
}
