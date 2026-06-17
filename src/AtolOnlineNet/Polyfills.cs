using System.Runtime.CompilerServices;

namespace AtolOnlineNet;

/// <summary>
/// Small argument-validation helpers used across the SDK. Provides a uniform null guard on every
/// target framework (the BCL <c>ArgumentNullException.ThrowIfNull</c> is not available on netstandard2.0).
/// </summary>
internal static class Guard
{
    /// <summary>Throws an <see cref="System.ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.</summary>
    public static T NotNull<T>(
        T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : class
    {
        if (argument is null)
        {
            throw new System.ArgumentNullException(paramName);
        }

        return argument;
    }

    /// <summary>Throws an <see cref="System.ArgumentException"/> if <paramref name="argument"/> is null, empty or whitespace.</summary>
    public static string NotNullOrWhiteSpace(
        string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new System.ArgumentException("Value cannot be null or whitespace.", paramName);
        }

        return argument!;
    }
}
