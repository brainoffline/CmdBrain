// See https://aka.ms/new-console-template for more information
using No8.CmdBrain;

Console.WriteLine( Figlet.Render("Allo, World!"));
await new Prog(new AltScreenModel()).Run();

public class AltScreenModel : Model
{
    public bool AltScreen { get; set; }
    public bool Quitting { get; set; }

    public Cmd? Init()
    {
        return null;
    }

    public (Model, Cmd?) Update(Msg msg)
    {
        switch(msg)
        {
            case KeyMsg keyMsg:
                switch(keyMsg.Key.Type)
                {
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        Quitting = true;
                        return (this, new QuitMsg().ToCmd());
                    case ConsoleKey.Spacebar:
                        AltScreen = !AltScreen;
                        if (AltScreen)
                            msg = new EnterAltScreenMsg();
                        else
                            msg = new ExitAltScreenMsg();
                        return (this, msg.ToCmd());
                }
                break;
        }
        return (this, null);
    }

    public string View()
    {
        if (Quitting)
            return "Bye!";

        var mode = AltScreen ? "AltScreen mode" : "Inline mode";

        return $"\n\n  You're in {mode}\n\n\n  space: switch modes | q : exit\n\n";
    }
}
