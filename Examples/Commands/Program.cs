using No8.CmdBrain;

Console.WriteLine( Figlet.Render("Allo, Commands!") );

var prog = new Prog(new model { });
await prog.Run();

Console.WriteLine("Laters!");

public record StatusMsg(string Status) : Msg;
public record ErrMsg(string Err) : Msg;

public class model : Model
{
    const string url = "https://charm.sh/";

    public string? Status { get; set; }
    public string? Err { get; set; }

    public async Task<Msg> CheckServer()
    {
        var client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        var res = await client.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return new ErrMsg(res.StatusCode.ToString());
        return new StatusMsg(res.StatusCode.ToString());
    }

    public Cmd? Init()
    {
        return CheckServer;
    }

    public (Model, Cmd?) Update(Msg msg)
    {
        switch(msg)
        {
            case StatusMsg statusMsg:
                Status = statusMsg.Status;
                break;
            case ErrMsg errMsg:
                Err = errMsg.Err;
                break;
            case KeyMsg keyMsg:
                if (keyMsg.Key.Type == ConsoleKey.Q)
                    return (this, new QuitMsg().ToCmd());
                break;
        }
        return (this, null);
    }

    public string View()
    {
        if (Err != null)
            return $"\nWe had some trouble: {Err}\n\n";

        return $"\nChecking {url} ... {Status}\n\n";
    }
}