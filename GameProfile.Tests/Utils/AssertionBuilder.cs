using System.Net;
using Xunit.Sdk;

namespace GameProfile.Tests.Utils;

public class AssertionBuilder
{
    public AssertionBuilder Equal<T>(T expected, T actual, string message = null)
    {
        try
        {
            Xunit.Assert.Equal(expected, actual);
        }
        catch (EqualException ex)
        {
            throw new XunitException(BuildMessage(message, expected, actual), ex);
        }
        return this;
    }

    public AssertionBuilder True(bool condition, string message = null)
    {
        try
        {
            Xunit.Assert.True(condition);
        }
        catch (TrueException ex)
        {
            throw new XunitException(message ?? "Condition was expected to be true but was false", ex);
        }
        return this;
    }

    public AssertionBuilder False(bool condition, string message = null)
    {
        try
        {
            Xunit.Assert.False(condition);
        }
        catch (FalseException ex)
        {
            throw new XunitException(message ?? "Condition was expected to be false but was true", ex);
        }
        return this;
    }

    public AssertionBuilder NotNull(object obj, string message = null)
    {
        try
        {
            Xunit.Assert.NotNull(obj);
        }
        catch (NotNullException ex)
        {
            throw new XunitException(message ?? "Object was expected to be not null but was null", ex);
        }
        return this;
    }

    public AssertionBuilder Null(object obj, string message = null)
    {
        try
        {
            Xunit.Assert.Null(obj);
        }
        catch (NullException ex)
        {
            throw new XunitException(message ?? "Object was expected to be null but was not null", ex);
        }
        return this;
    }

    public AssertionBuilder NotNullOrEmpty(string str, string message = null)
    {
        try
        {
            Xunit.Assert.NotNull(str);
            Xunit.Assert.NotEmpty(str);
        }
        catch (Exception ex) when (ex is NotNullException or NotEmptyException)
        {
            throw new XunitException(message ?? $"Expected string to be not null and not empty but was: '{str}'", ex);
        }
        return this;
    }

    public AssertionBuilder NotEmpty<T>(IEnumerable<T> collection, string message = null)
    {
        try
        {
            Xunit.Assert.NotEmpty(collection);
        }
        catch (NotEmptyException ex)
        {
            throw new XunitException(message ?? "Expected collection to not be empty but it was", ex);
        }
        return this;
    }

    public AssertionBuilder Count<T>(IEnumerable<T> collection, int expected, string message = null)
    {
        var actual = collection?.Count() ?? 0;
        try
        {
            Xunit.Assert.Equal(expected, actual);
        }
        catch (EqualException ex)
        {
            throw new XunitException(
                message ?? $"Expected collection of {expected} items but got {actual}", ex);
        }
        return this;
    }

    public AssertionBuilder Contains(string expectedSubstring, string actualString, string message = null)
    {
        try
        {
            Xunit.Assert.Contains(expectedSubstring, actualString);
        }
        catch (ContainsException ex)
        {
            throw new XunitException(
                message ?? $"Expected string to contain '{expectedSubstring}' but it was '{actualString}'", ex);
        }
        return this;
    }

    public AssertionBuilder SortedAscendingByName<T>(
        IEnumerable<T> items,
        Func<T, string> nameSelector,
        string message = null,
        StringComparer comparer = null)
    {
        var cmp = comparer ?? StringComparer.Ordinal;
        var list = items?.ToList() ?? new List<T>();
        for (var i = 1; i < list.Count; i++)
        {
            var prev = nameSelector(list[i - 1]);
            var curr = nameSelector(list[i]);
            if (cmp.Compare(prev, curr) > 0)
            {
                throw new XunitException(
                    message ?? $"Not sorted ascending by name at index {i}: '{prev}' > '{curr}'.");
            }
        }
        return this;
    }

    private static string BuildMessage(string message, object expected, object actual) =>
        message != null
            ? $"{message}\nExpected: {expected}, Actual: {actual}"
            : $"Expected: {expected}, Actual: {actual}";
}

public static class Assertions
{
    public static AssertionBuilder Validate() => new();
}
