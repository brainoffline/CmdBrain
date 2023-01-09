using No8.CmdBrain.VirtualTerminal;
using System.Collections.Concurrent;

namespace No8.CmdBrain;

public delegate void ProgramOption(Prog prog);

[Flags]
public enum StartupOptions
{
    None = 0x0,
    withAltScreen = 0x01,
    withMouseCellMotion = 0x02,
    withMouseAllMotion = 0x04,
    withANSICompressor = 0x08,
    withoutSignalHandler = 0x10,
}

public partial class Prog
{
    StartupOptions StartupOptions;
    Renderer? Renderer = null;

    private Ctx? _ctx;
    public Ctx Ctx => _ctx ??= new();

    public Model Model { get; set; }

    public ConcurrentQueue<Msg> Msgs { get; } = new();
    public ConcurrentQueue<Cmd> Cmds { get; } = new();

    public event EventHandler? Cancel;
    public void RaiseCancel() => Cancel?.Invoke(this, EventArgs.Empty);

    private TextReader? _in = null;
    public TextReader In
    {
        get => _in = _in ?? Console.In;
        set => _in = value;
    }

    private TextWriter? _out = null;
    public TextWriter Out
    {
        get => _out = _out ?? Console.Out;
        set => _out = value;
    }

    private TextWriter? _err = null;
    public TextWriter Err
    {
        get => _err = _err ?? Console.Error;
        set => _err = value;
    }

    public Action? RestoreOutput;

    bool altScreenWasActive;

    public Prog(Model model, StartupOptions options = StartupOptions.None)
    {
        In ??= Console.In;
        Out ??= Console.Out;
        Err ??= Console.Error;
        Model = model;
        StartupOptions = options;
    }

    public void EnterAltScreen() => Renderer?.EnterAltScreen();
    public void ExitAltScreen() => Renderer?.ExitAltScreen();

    public void EnableMouseCellMotion() => Renderer?.EnableMouseCellMotion();
    public void EnableMouseAllMotion() => Renderer?.EnableMouseAllMotion();
    public void DisableMouse() => Renderer?.DisableMouseAllMotion();
    public void HideCursor() => Renderer?.HideCursor();
    public void ShowCursor() => Renderer?.ShowCursor();

    public async Task Run()
    {
        var origEncoding = Console.OutputEncoding;
        Console.OutputEncoding = Encoding.UTF8; // enable emoji's!        

        Con.SequenceAvailable += Con_SequenceAvailable;
        Con.KeyAvailable += Con_KeyAvailable;
        //Con.Send(Terminal.Mode.MouseTrackingAll);
        //Con.Start();

        Renderer ??= new StandardRenderer()
        {
            Width = Console.WindowWidth,
            Height = Console.WindowHeight
        };

        HideCursor();

        if (StartupOptions.HasFlag(StartupOptions.withAltScreen))
            EnterAltScreen();
        if (StartupOptions.HasFlag(StartupOptions.withMouseCellMotion))
            EnableMouseCellMotion();
        if (StartupOptions.HasFlag(StartupOptions.withMouseAllMotion))
            EnableMouseAllMotion();

        var renderTask = Renderer.Start();

        var initCmd = Model.Init();
        if (initCmd != null)
            Send(initCmd);

        Renderer.Write(Model.View());

        await Task.WhenAll(
            //            Task.Run(HandleCmds),
            Task.Run(HandleMsgs)
            );
        Con.SequenceAvailable -= Con_SequenceAvailable;

        Con.Mode.StopMouseTrackingAll();
        Console.OutputEncoding = origEncoding;
        if (Renderer?.AltScreen == true)
        {
            Renderer.ExitAltScreen();
            await Task.Delay(10);
        }
        ShowCursor();
    }

    private void Con_KeyAvailable(object? sender, ConsoleKeyInfo e)
    {
        Trace.WriteLine($"KEY: {e.Key} {e.KeyChar} {e.Modifiers}");
        var isAlt = e.Modifiers.HasFlag(ConsoleModifiers.Alt);
        Send(new KeyMsg(new Key(e.Key, null, isAlt)));
    }

    private void Con_SequenceAvailable(object? sender, string str)
    {
        Trace.WriteLine("SEQ: " + str);
    }

    /// <summary>
    ///     Trigger Running prog to stop
    /// </summary>
    public void Stop()
    {
        Con.Stop();
    }

    public void Send(Msg msg) => Msgs.Enqueue(msg);
    public void Send(Cmd cmd) => Cmds.Enqueue(cmd);

    /*
    private async Task HandleCmds()
    {
        while (Con.MonitoringInput)
        {
            if (Cmds.TryDequeue(out var cmd))
            {
                var msg = await cmd();
                if (msg != null)
                    Send(msg);
            }
            else
                await Task.Delay(50);
        }
    }
    */

    private async Task HandleMsgs()
    {
        while (Con.MonitoringInput)
        {
            if (Msgs.TryDequeue(out var msg))
            {
                switch (msg)
                {
                    case QuitMsg _: Stop(); break;
                    case ClearScreenMsg _: Renderer?.ClearScreen(); break;
                    case EnterAltScreenMsg _: Renderer?.EnterAltScreen(); break;
                    case ExitAltScreenMsg _: Renderer?.ExitAltScreen(); break;

                    case EnableMouseCellMotionMsg _: Renderer?.EnableMouseCellMotion(); break;
                    case EnableMouseAllMotionMsg _: Renderer?.EnableMouseAllMotion(); break;
                    case DisableMouseMsg _: Renderer?.DisableMouseAllMotion(); break;

                    case ShowCursorMsg _: Renderer?.ShowCursor(); break;
                    case HideCursorMsg _: Renderer?.HideCursor(); break;

                    default:
                        if (Renderer is StandardRenderer stdRenderer)
                            stdRenderer.handleMessages(msg);

                        var (model, cmd) = Model.Update(msg);
                        if (cmd != null)
                            Send(cmd);
                        Renderer?.Write(Model.View());
                        break;
                }
                continue;
            }
            else if (Cmds.TryDequeue(out var cmd))
            {
                var cmdMsg = await cmd();
                if (cmdMsg != null)
                    Send(cmdMsg);
                continue;
            }
            else
                await Task.Delay(50);
        }
    }

    public void Println(string[] strings)
    {
        Send(new PrintLineMsg(string.Join("", strings)));
    }

}


