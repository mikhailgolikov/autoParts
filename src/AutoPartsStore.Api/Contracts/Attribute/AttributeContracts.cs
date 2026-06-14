using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Api.Contracts.Attribute;

public record AttributeResponse(
    Guid Id,
    string Name,
    AttributeType Type,
    string? Unit,
    Guid CategoryId);

public record CreateAttributeRequest(
    string Name,
    AttributeType Type,
    string? Unit,
    Guid CategoryId);

public record UpdateAttributeRequest(
    string Name,
    AttributeType Type,
    string? Unit);
