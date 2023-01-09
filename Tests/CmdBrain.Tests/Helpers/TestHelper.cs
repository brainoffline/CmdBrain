namespace No8.CmdBrain.Tests.Helpers;

public static class TestHelper
{
    /// <summary>
    /// Runs multiple checks, collecting the exceptions from each one, and then bundles all failures
    /// up into a single assertion failure.
    /// </summary>
    /// <param name="checks">The individual assertions to run, as actions.</param>
    public static void AssertMultiple(params Action[] checks)
    {
        if (checks == null || checks.Length == 0)
            return;

        var exceptions = new List<Exception>();

        foreach (var check in checks)
            try
            {
                check();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

        if (exceptions.Count == 0)
            return;
        if (exceptions.Count == 1)
            throw exceptions.First();
            //ExceptionDispatchInfo.Capture(exceptions[0]).Throw();

        throw new AggregateException(exceptions);
    }

    public static void AssertException<T>(Type exception, string message, T a, Action<T> action)
    {
        Type? actualType = null;
        string? stack = null;
        string? actualMessage = null;
        try
        {
            action(a);
        }
        catch (Exception e)
        {
            actualType = e.GetType();
            actualMessage = e.Message;
            if (actualType != exception)
                stack = e.ToString();
        }

        if (actualType != exception)
        {
            throw new InvalidOperationException(
                $"Assertion failed: Expected Exception Type {exception}, got {actualType}.\n" + $"Actual Exception: {stack}");
        }

        if (actualMessage != message)
            throw new InvalidOperationException(
                $"Assertion failed:\n\tExpected: {message}\n\t  Actual: {actualMessage}");
    }

    public static void AssertException(Type exception, string message, Action action)
    {
        Type?   actualType    = null;
        string? stack         = null;
        string? actualMessage = null;

        try
        {
            action();
        }
        catch (Exception e)
        {
            actualType    = e.GetType();
            actualMessage = e.Message;
            if (actualType != exception)
                stack = e.ToString();
        }

        if (actualType != exception)
        {
            throw new InvalidOperationException(
                $"Assertion failed: Expected Exception Type {exception}, got {actualType}.\n" + $"Actual Exception: {stack}");
        }

        if (actualMessage != message)
            throw new InvalidOperationException(
                $"Assertion failed:\n\tExpected: {message}\n\t  Actual: {actualMessage}");
    }


}
