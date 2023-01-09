namespace No8.CmdBrain;

public record KeyMsg(Key Key) : Msg { }

public record Key (ConsoleKey Type, Rune[]? Runes = null, bool Alt = false)
{
    public const ConsoleKey KeyRunes = (ConsoleKey)0x0FFF;

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Alt) sb.Append("alt+");
        if (Type == KeyRunes)
        {
            if (Runes != null)
            {
                foreach (var rune in Runes)
                    sb.Append(rune.ToString());
            }
        }
        else
            sb.Append(Type);

        return sb.ToString();
    }
}


