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
    }
}