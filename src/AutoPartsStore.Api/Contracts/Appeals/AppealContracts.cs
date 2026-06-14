using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Api.Contracts.Appeals;

public record ClientQuestionResponse(
    Guid Id,
    string Number,
    DateTime CreatedAt,
    Guid UserId,
    string ContactPhone,
    string ContactEmail,
    string ManagerComment,
    AppealStatus Status,
    AppealCategory Category);

public record SupplierRequestResponse(
    Guid Id,
    string Number,
    DateTime CreatedAt,
    Guid UserId,
    string ContactPhone,
    string ContactEmail,
    string ManagerComment,
    AppealStatus Status,
    string CompanyName);

public record CreateClientQuestionRequest(
    AppealCategory Category,
    string ManagerComment,
    string? ContactPhone = null,
    string? ContactEmail = null);

public record CreateSupplierRequestRequest(
    string CompanyName,
    string ManagerComment,
    string? ContactPhone = null,
    string? ContactEmail = null);

public record UpdateAppealStatusRequest(AppealStatus Status);

public record UpdateAppealContactsRequest(
    string ContactPhone,
    string ContactEmail);
