using System.Globalization;
using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Domain.Helpers;

public static class AttributeValueParser
{
    public static string NormalizeToString(string value, AttributeType type) =>
        type switch
        {
            AttributeType.Bool => NormalizeBool(value),
            AttributeType.Int => NormalizeInt(value),
            AttributeType.Float => NormalizeFloat(value),
            _ => value.Trim()
        };

    public static int? TryParseAsInt(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;

    public static double? TryParseAsFloat(string value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;

    public static bool? TryParseAsBool(string value)
    {
        if (bool.TryParse(value, out var result))
        {
            return result;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "1" or "yes" or "да" or "true" => true,
            "0" or "no" or "нет" or "false" => false,
            _ => null
        };
    }

    public static bool IsValidForType(string value, AttributeType type) =>
        type switch
        {
            AttributeType.Int => TryParseAsInt(value).HasValue,
            AttributeType.Float => TryParseAsFloat(value).HasValue,
            AttributeType.Bool => TryParseAsBool(value).HasValue,
            _ => !string.IsNullOrWhiteSpace(value)
        };

    public static bool MatchesFilter(string value, AttributeType type, string? min, string? max, string? exact)
    {
        return type switch
        {
            AttributeType.Int when TryParseAsInt(value) is int intVal =>
                MatchesRange(intVal, min is not null ? TryParseAsInt(min) : null, max is not null ? TryParseAsInt(max) : null)
                && (exact is null || value.Equals(exact, StringComparison.OrdinalIgnoreCase)),
            AttributeType.Float when TryParseAsFloat(value) is double floatVal =>
                MatchesRange(floatVal, min is not null ? TryParseAsFloat(min) : null, max is not null ? TryParseAsFloat(max) : null)
                && (exact is null || value.Equals(exact, StringComparison.OrdinalIgnoreCase)),
            AttributeType.Bool when TryParseAsBool(value) is bool boolVal =>
                exact is null || TryParseAsBool(exact) == boolVal,
            _ => exact is null || value.Contains(exact, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static string NormalizeInt(string value) =>
        TryParseAsInt(value.Trim())?.ToString(CultureInfo.InvariantCulture)
        ?? throw new FormatException("Invalid integer value.");

    private static string NormalizeFloat(string value) =>
        TryParseAsFloat(value.Trim())?.ToString(CultureInfo.InvariantCulture)
        ?? throw new FormatException("Invalid float value.");

    private static string NormalizeBool(string value) =>
        TryParseAsBool(value.Trim())?.ToString().ToLowerInvariant()
        ?? throw new FormatException("Invalid boolean value.");

    private static bool MatchesRange<T>(T value, T? min, T? max) where T : struct, IComparable<T>
    {
        if (min.HasValue && value.CompareTo(min.Value) < 0) return false;
        if (max.HasValue && value.CompareTo(max.Value) > 0) return false;
        return true;
    }
}
