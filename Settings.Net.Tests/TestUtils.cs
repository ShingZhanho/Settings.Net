#nullable enable
using System;

namespace Settings.Net.Tests
{
    public class TestUtils
    {
        internal static Exception? GetExceptionFromFunction<TResult>(Func<object?, TResult> function, object? parameters, out TResult? result)
        {
            try
            {
                result = function.Invoke(parameters);
            }
            catch (Exception e)
            {
                result = default;
                return e;
            }
            return null;
        }

        internal static Exception? GetExceptionFromFunction(Action<object[]> action, object[]? parameters)
        {
            parameters ??= Array.Empty<object>();
            try
            {
                action(parameters);
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }
    }
}