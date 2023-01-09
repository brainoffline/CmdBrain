using System.Diagnostics;
using System.Text;
using No8.CmdBrain;

Console.WriteLine("Allo, Basics!");

var model = new BasicsModel
{
    choices = new() { "Buy carrots", "Buy celery", "Buy kohlrabi" }
};
await new Prog(model).Run();



public class BasicsModel : Model {

    public int cursor;
    public List<string> choices = new();
    public Dictionary<int, object> selected = new();

    public Cmd? Init()
    {
        return null;
    }

    public (Model, Cmd?) Update(Msg msg)
    {
        Cmd? cmd = null;
        switch(msg)
        {
            case KeyMsg keyMsg:
                cmd = HandleKey(keyMsg.Key);
                break;
        }

        return (this, cmd);
    }

    private Cmd? HandleKey(Key key)
    {
        Trace.WriteLine(key);
        switch(key.Type)
        {
            case ConsoleKey.Q:
                return new QuitMsg().ToCmd();
            case ConsoleKey.UpArrow:
            case ConsoleKey.K:
                if (cursor > 0)
                    cursor--;
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.J:
                if (cursor < choices.Count - 1)
                    cursor++ ;
                break;
            case ConsoleKey.Spacebar:
            case ConsoleKey.Enter:
                if (selected.ContainsKey(cursor))
                    selected.Remove(cursor);
                else
                    selected[cursor] = true;
                break;
        }
        return null;
    }

    public string View()
    {
        var sb = new StringBuilder( "What should I buy at the market?\n\n" );
        foreach (var (choice, i) in choices.WithIndex())
        {
            var c = " ";
            if (cursor == i)
                c = ">";

            var check = " ";
            if (selected.ContainsKey(i))
                check = "x";

            sb.AppendLine($"{c} [{check}] {choice}");
        }

        sb.AppendLine();
        sb.AppendLine("Press q to quit.");
        return sb.ToString();
    }
}
