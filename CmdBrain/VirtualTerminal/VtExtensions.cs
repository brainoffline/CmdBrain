namespace No8.CmdBrain.VirtualTerminal;

public static class VtExtensions
{

    // ReSharper disable once InconsistentNaming
    public static bool IsSS3( this VtCommand cmd ) =>
        cmd.Action == VtAction.EscDispatch && cmd.Ch == 'O';

    public static VtModifiers ToVtModifiers( this ConsoleModifiers modifier )
    {
        VtModifiers result = 0;
        if (modifier.HasFlag(ConsoleModifiers.Shift))
            result |= VtModifiers.Shift;
        if (modifier.HasFlag(ConsoleModifiers.Alt))
            result |= VtModifiers.Alt;
        if (modifier.HasFlag(ConsoleModifiers.Control))
            result |= VtModifiers.Control;

        return result;
    }
}