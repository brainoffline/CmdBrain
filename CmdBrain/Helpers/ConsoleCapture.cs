namespace No8.CmdBrain;

public class ConsoleCapture : IDisposable
{
    public readonly  StringBuilder StringBuilder = new();
    private readonly TextWriter    _prevOut;
    private readonly TextWriter    _prevError;

    public ConsoleCapture()
    {
        _prevOut   = System.Console.Out;
        _prevError = System.Console.Error;

        var stringWriter = new StringWriter(StringBuilder);
        System.Console.SetOut(stringWriter);
        System.Console.SetError(stringWriter);
    }

    public string NextString()
    {
        var str = StringBuilder.ToString();
        StringBuilder.Clear();
        return str;
    }

    public void Dispose()
    {
        System.Console.SetOut(_prevOut);
        System.Console.SetError(_prevError);
    }
}