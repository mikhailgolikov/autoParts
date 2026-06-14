using AutoPartsStore.Api.Contracts.Users;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Domain.Enums;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class UserService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => ToResponse(u))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null ? null : ToResponse(user);
    }

    public async Task<UserResponse> CreateByAdminAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
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
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.Name = request.Name.Trim();
        user.PhoneNumber = request.PhoneNumber?.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(user);
    }

    public async Task<UserResponse?> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (email != user.Email &&
            await dbContext.Users.AnyAsync(u => u.Email == email && u.Id != userId, cancellationToken))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        user.Name = request.Name.Trim();
        user.Email = email;
        user.PhoneNumber = request.PhoneNumber?.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(user);
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserResponse?> ChangeRoleAsync(
        Guid userId,
        ChangeUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (user.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Admin role cannot be changed.");
        }

        if (request.Role is not (UserRole.Client or UserRole.Creator))
        {
            throw new InvalidOperationException("Role can only be changed between Client and Creator.");
        }

        if (user.Role == request.Role)
        {
            return ToResponse(user);
        }

        user.Role = request.Role;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(user);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid adminId, CancellationToken cancellationToken = default)
    {
        if (userId == adminId)
        {
            throw new InvalidOperationException("You cannot delete your own account.");
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        if (user.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Admin accounts cannot be deleted.");
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Name, user.Email, user.PhoneNumber, user.Role, user.CreatedAt);
}
