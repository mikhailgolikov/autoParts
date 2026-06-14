using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Appeals;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Domain.Enums;
using AutoPartsStore.Infrastructure.Persistence;
using AutoPartsStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class AppealService(
    AppDbContext dbContext,
    AppealNumberGenerator appealNumberGenerator,
    ICurrentUserService currentUser)
{
    public async Task<IReadOnlyList<object>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureAdminOrCreator();

        var appeals = await dbContext.Appeals
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return appeals.Select(ToResponse).Cast<object>().ToList();
    }

    public async Task<IReadOnlyList<object>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        EnsureCanAccessUserAppeals(userId);

        var appeals = await dbContext.Appeals
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return appeals.Select(ToResponse).Cast<object>().ToList();
    }

    public async Task<object?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appeal = await dbContext.Appeals
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (appeal is null)
        {
            return null;
        }

        EnsureCanAccessAppeal(appeal);
        return ToResponse(appeal);
    }

    public async Task<ClientQuestionResponse> CreateClientQuestionAsync(
        CreateClientQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = EnsureAuthenticated();
        var user = await GetUserOrThrowAsync(userId, cancellationToken);
        var contacts = ResolveContacts(user, request.ContactPhone, request.ContactEmail);

        var appeal = new ClientQuestion
        {
            Id = Guid.NewGuid(),
            Number = await appealNumberGenerator.GenerateNextAsync(cancellationToken),
            UserId = userId,
            ContactPhone = contacts.Phone,
            ContactEmail = contacts.Email,
            ManagerComment = request.ManagerComment.Trim(),
            Category = request.Category,
            Status = AppealStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.ClientQuestions.Add(appeal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToClientQuestionResponse(appeal);
    }

    public async Task<SupplierRequestResponse> CreateSupplierRequestAsync(
        CreateSupplierRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = EnsureAuthenticated();
        var user = await GetUserOrThrowAsync(userId, cancellationToken);
        var contacts = ResolveContacts(user, request.ContactPhone, request.ContactEmail);

        var appeal = new SupplierRequest
        {
            Id = Guid.NewGuid(),
            Number = await appealNumberGenerator.GenerateNextAsync(cancellationToken),
            UserId = userId,
            ContactPhone = contacts.Phone,
            ContactEmail = contacts.Email,
            ManagerComment = request.ManagerComment.Trim(),
            CompanyName = request.CompanyName.Trim(),
            Status = AppealStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.SupplierRequests.Add(appeal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToSupplierRequestResponse(appeal);
    }

    public async Task<object?> UpdateStatusAsync(
        Guid id,
        UpdateAppealStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdminOrCreator();

        var appeal = await dbContext.Appeals.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (appeal is null)
        {
            return null;
        }

        appeal.Status = request.Status;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(appeal);
    }

    public async Task<object?> UpdateContactsAsync(
        Guid id,
        UpdateAppealContactsRequest request,
        CancellationToken cancellationToken = default)
    {
        var appeal = await dbContext.Appeals.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (appeal is null)
        {
            return null;
        }

        EnsureCanModifyAppeal(appeal);

        appeal.ContactPhone = request.ContactPhone.Trim();
        appeal.ContactEmail = request.ContactEmail.Trim().ToLowerInvariant();
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(appeal);
    }

    private Guid EnsureAuthenticated()
    {
        return currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authentication required.");
    }

    private void EnsureAdminOrCreator()
    {
        if (!currentUser.IsAdminOrCreator)
        {
            throw new UnauthorizedAccessException("Admin or Creator role required.");
        }
    }

    private void EnsureCanAccessUserAppeals(Guid userId)
    {
        if (currentUser.IsAdminOrCreator || currentUser.UserId == userId)
        {
            return;
        }

        throw new UnauthorizedAccessException("You can only view your own appeals.");
    }

    private void EnsureCanAccessAppeal(Appeal appeal)
    {
        if (currentUser.IsAdminOrCreator || currentUser.UserId == appeal.UserId)
        {
            return;
        }

        throw new UnauthorizedAccessException("You can only view your own appeals.");
    }

    private void EnsureCanModifyAppeal(Appeal appeal)
    {
        if (currentUser.IsAdminOrCreator || currentUser.UserId == appeal.UserId)
        {
            return;
        }

        throw new UnauthorizedAccessException("You can only modify your own appeals.");
    }

    private async Task<User> GetUserOrThrowAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");
    }

    private static (string Phone, string Email) ResolveContacts(
        User user,
        string? contactPhone,
        string? contactEmail)
    {
        var phone = string.IsNullOrWhiteSpace(contactPhone)
            ? user.PhoneNumber
            : contactPhone.Trim();

        var email = string.IsNullOrWhiteSpace(contactEmail)
            ? user.Email
            : contactEmail.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new InvalidOperationException("Contact phone is required. Set it in profile or in the request.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Contact email is required. Set it in profile or in the request.");
        }

        return (phone, email);
    }

    private static object ToResponse(Appeal appeal) =>
        appeal switch
        {
            ClientQuestion clientQuestion => ToClientQuestionResponse(clientQuestion),
            SupplierRequest supplierRequest => ToSupplierRequestResponse(supplierRequest),
            _ => throw new InvalidOperationException("Unknown appeal type.")
        };

    private static ClientQuestionResponse ToClientQuestionResponse(ClientQuestion appeal) =>
        new(
            appeal.Id,
            appeal.Number,
            appeal.CreatedAt,
            appeal.UserId,
            appeal.ContactPhone,
            appeal.ContactEmail,
            appeal.ManagerComment,
            appeal.Status,
            appeal.Category);

    private static SupplierRequestResponse ToSupplierRequestResponse(SupplierRequest appeal) =>
        new(
            appeal.Id,
            appeal.Number,
            appeal.CreatedAt,
            appeal.UserId,
            appeal.ContactPhone,
            appeal.ContactEmail,
            appeal.ManagerComment,
            appeal.Status,
            appeal.CompanyName);
}
