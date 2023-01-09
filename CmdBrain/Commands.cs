namespace No8.CmdBrain;

public interface Msg { }
public delegate Task<Msg> Cmd();

public record QuitMsg : Msg { }

public record WindowSizeMsg(int Width, int Height) : Msg { }
public record ClearScreenMsg : Msg { }
public record EnterAltScreenMsg : Msg { }
public record ExitAltScreenMsg : Msg { }

public record EnableMouseCellMotionMsg : Msg { }
public record EnableMouseAllMotionMsg : Msg { }
public record DisableMouseMsg : Msg { }
public record HideCursorMsg : Msg { }
public record ShowCursorMsg : Msg { }

public static class MsgHelper
{
    public static Cmd ToCmd(this Msg msg) =>
        () => Task.FromResult(msg);
}


public abstract class CommandArray : Msg
{
    protected readonly Cmd[] _cmds;

    public CommandArray(IEnumerable<Cmd> cmds)
    {
        _cmds = cmds.ToArray();
    }
}

public class ConcurrentMsgs : CommandArray
{
    public ConcurrentMsgs(IEnumerable<Cmd> cmds) : base(cmds) { }
}

public class SequentialMsgs : CommandArray
{
    public SequentialMsgs(IEnumerable<Cmd> cmds) : base(cmds) { }
}


public static partial class Commands
{
    /// <summary>
    ///     Wrap an array of commands into a single Cmd.
    ///     Cmds can be executes concurrently
    /// </summary>
    public static Cmd Concurrent(params Cmd[] cmds) =>
        new ConcurrentMsgs(cmds).ToCmd();

    /// <summary>
    ///     Wrap an array of commands into a Sequential Cmd.
    ///     Cmds will be executed in sequence
    /// </summary>
    public static Cmd Sequence(params Cmd[] cmds) =>
        new SequentialMsgs(cmds).ToCmd();

    public static Cmd Tick(TimeSpan duration, Func<DateTime, Msg> func)
    {
        return async () =>
        {
            await Task.Delay(duration);
            return func(DateTime.Now);
        };
    }
}
