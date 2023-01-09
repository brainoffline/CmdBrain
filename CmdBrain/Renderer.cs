using No8.CmdBrain.VirtualTerminal;

namespace No8.CmdBrain;

public record RepaintMsg : Msg { }
public record ClearScrollAreaMsg : Msg { }
public record SyncScrollAreaMsg(
    IEnumerable<string> lines,
    int topBoundary,
    int bottomBoundary
    ) : Msg { }
public record ScrollUpMsg(
    IEnumerable<string> Lines,
    int TopBoundary,
    int BottomBoundary
    ) : Msg { }
public record ScrollDownMsg (
    IEnumerable<string> Lines,
    int TopBoundary,
    int BottomBoundary
    ): Msg { }

public record PrintLineMsg (
    string MessageBody
    ) : Msg { }

public interface Renderer
{
    // Start the renderer.
    Task Start();

    // Stop the renderer, but render the final frame in the buffer, if any.
    void Stop();

    // Stop the renderer without doing any final rendering.
    void Kill();

    // Write a frame to the renderer. The renderer can write this data to
    // output at its discretion.
    void Write(string str);

    // Request a full re-render. Note that this will not trigger a render
    // immediately. Rather, this method causes the next render to be a full
    // repaint. Because of this, it's safe to call this method multiple times
    // in succession.
    void Repaint();

    // Clears the terminal.
    void ClearScreen();

    // Whether or not the alternate screen buffer is enabled.
    bool AltScreen { get; }
    // Enable the alternate screen buffer.
    void EnterAltScreen();
    // Disable the alternate screen buffer.
    void ExitAltScreen();

    // Show the cursor.
    void ShowCursor();
    // Hide the cursor.
    void HideCursor();

    // enableMouseCellMotion enables mouse click, release, wheel and motion
    // events if a mouse button is pressed (i.e., drag events).
    void EnableMouseCellMotion();

    // DisableMouseCellMotion disables Mouse Cell Motion tracking.
    void DisableMouseCellMotion();
    
    // EnableMouseAllMotion enables mouse click, release, wheel and motion
    // events, regardless of whether a mouse button is pressed. Many modern
    // terminals support this, but not all.
    void EnableMouseAllMotion();

    // DisableMouseAllMotion disables All Motion mouse tracking.
    void DisableMouseAllMotion();
}

public class NullRenderer : Renderer
{
    public bool AltScreen => false;
    public void ClearScreen() { }
    public void DisableMouseAllMotion() { }
    public void DisableMouseCellMotion() { }
    public void EnableMouseAllMotion() { }
    public void EnableMouseCellMotion() { }
    public void EnterAltScreen() { }
    public void ExitAltScreen() { }
    public void HideCursor() { }
    public void Kill() { }
    public void Repaint() { }
    public void ShowCursor() { }
    public Task Start() => Task.CompletedTask;
    public void Stop() { }
    public void Write(string str) { }
}

public class StandardRenderer : Renderer
{
    private static readonly TimeSpan defaultFrameRate = TimeSpan.FromSeconds(1) / 60;

    public bool CursorHidden { get; private set; }
    public bool AltScreen { get; private set; }

    // renderer dimensions; usually the size of the window
    public int Width { get; set; }
    public int Height { get; set; }


    private TaskCompletionSource? _rendererStarted;
    private TimeSpan _frameRate = defaultFrameRate;
    private Timer? _timer;
    private StringBuilder _sb = new();
    private string? _lastRender;
    private List<string> _queuedMessageLines = new();
    private Dictionary<int, object> _ignoreLines = new();
    private int _linesRendered;

    private object _renderLock = new();

    public Task Start()
    {
        _rendererStarted?.SetResult();
        _rendererStarted = new();

        Con.Start();

        if (_timer == null)
            _timer = new Timer(Flush, null, TimeSpan.Zero, _frameRate);

        return _rendererStarted.Task;
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;

        Flush(null);

        _rendererStarted?.SetResult();
        _rendererStarted = null;

        Con.Stop();
    }

    public void Kill()
    {
        _rendererStarted?.SetResult();

        Con.Stop();
    }

    public void Write(string str) 
    {
        _sb.Append(str);
    }

    public void Writeline(string? str = null)
    {
        _sb.AppendLine(str);
    }

    public void Repaint() 
    {
        _lastRender = null;
    }
    public void ClearScreen() 
    {
        lock (_renderLock)
        {
            Con.ClearScreen();
            Con.Cursor.Set(1, 1);
        }
        Repaint();
    } 

    public void EnterAltScreen()
    {
        if (AltScreen)
            return;
        AltScreen = true;
        Repaint();
    }

    public void ExitAltScreen()
    {
        if (!AltScreen) 
            return;
        AltScreen = false;
        Repaint();
    }

    public void HideCursor() => CursorHidden = true;
    public void ShowCursor() => CursorHidden = false;

    public void DisableMouseAllMotion() => Con.Mode.StopMouseTrackingAll();
    public void DisableMouseCellMotion() => Con.Mode.StopMouseTrackingCell();
    public void EnableMouseAllMotion() => Con.Mode.MouseTrackingAll();
    public void EnableMouseCellMotion() => Con.Mode.MouseTrackingCell();

    private void Flush(object? state)
    {
        lock(_renderLock)
        {
            var buf = _sb.ToString();
            _sb.Clear();

            if (buf.Length == 0 || buf == _lastRender)
                return; // nothing to do

            Con.Cursor.Hidden = true;
            if (Con.Mode.AltScreen != AltScreen)
            {
                Con.Mode.AltScreen = AltScreen;
                Con.ClearScreen();
                Con.Cursor.Set(1, 1);
                _lastRender = "";
                Width = Console.WindowWidth; 
                Height = Console.WindowHeight;
            }

            var lines = buf.Split('\n').ToList();

            // If we know the output's height, we can use it to determine how many
            // lines we can render. We drop lines from the top of the render buffer if
            // necessary, as we can't navigate the cursor into the terminal's scrollback
            // buffer.
            if (Height > 0 && lines.Count > Height) 
                lines = lines.TakeLast(Height).ToList();

            var numLinesThisFlush = lines.Count;
            var oldLines = (_lastRender ?? "").Split();
            var skipLines = new Dictionary<int, object>();
            var flushQueuedMessages = _queuedMessageLines.Count > 0 && !AltScreen;

            if (flushQueuedMessages)
            {
                foreach (var line in _queuedMessageLines)
                    lines.Append(line);
                _queuedMessageLines.Clear();
            }

            // Clear any lines we painted in the last render.
            if (_linesRendered > 0)
            {
                for(int i = _linesRendered - 1; i > 0; i--)
                {
                    // If the number of lines we want to render hasn't increased and
                    // new line is the same as the old line we can skip rendering for
                    // this line as a performance optimization.
                    if ((lines.Count <= oldLines.Length) &&
                        (lines.Count > i && oldLines.Length > i) &&
                        (lines[i] == oldLines[i]))
                    {
                        skipLines[i] = new();
                    }
                    else if (_ignoreLines.ContainsKey(i))
                    {
                        // Clear Line
                        Con.EditingControlFunctions.EraseInLine(1);
                    }
                    Con.Cursor.Up(1);
		        }

                if (_ignoreLines.ContainsKey(0))
                {
                    // We need to return to the start of the line here to properly
                    // erase it. Going back the entire width of the terminal will
                    // usually be farther than we need to go, but terminal emulators
                    // will stop the cursor at the start of the line as a rule.
                    //
                    // We use this sequence in particular because it's part of the ANSI
                    // standard (whereas others are proprietary to, say, VT100/VT52).
                    // If cursor previous line (ESC[ + <n> + F) were better supported
                    // we could use that above to eliminate this step.
                    Con.Cursor.Left(Width);
                    Con.EditingControlFunctions.EraseInLine(1);
                }
            }

            // Merge the set of lines we're skipping as a rendering optimization with
            // the set of lines we've explicitly asked the renderer to ignore.
            if (_ignoreLines.Count > 0)
            {
                foreach (var num in _ignoreLines.Keys)
                    skipLines[num] = new();
            }

            // Paint new lines
            for (int i = 0; i < lines.Count; i++)
            {
                if (skipLines.ContainsKey(i))
                {
                    // Unless this is the last line, move the cursor down.
                    if (i < lines.Count - 1)
                    {
                        Con.Cursor.Down(1);
                    }
                } 
                else
                {
                    var line = lines[i];

                    // Truncate lines wider than the width of the window to avoid
                    // wrapping, which will mess up rendering. If we don't have the
                    // width of the window this will be ignored.
                    //
                    // Note that on Windows we only get the width of the window on
                    // program initialization, so after a resize this won't perform
                    // correctly (signal SIGWINCH is not supported on Windows).
                    if (Width > 0 && line.Length > Width) 
                    {
                        line = line.Substring(0, Width);
                    }

                    Con.Send(line);

                    if (i < lines.Count - 1) 
                        Con.Send("\r\n");
                }
            }
            _linesRendered = numLinesThisFlush;

            // Make sure the cursor is at the start of the last line to keep rendering
            // behavior consistent.
            if (AltScreen) 
            {
                // This case fixes a bug in macOS terminal. In other terminals the
                // other case seems to do the job regardless of whether or not we're
                // using the full terminal window.
                Con.Cursor.Set(_linesRendered, 0);
            }
            else
            {
                Con.Cursor.Left(Width);
            }
            Con.Cursor.Hidden = CursorHidden;

            _lastRender = buf;
        }
    }

    private void setIgnoredLines(int from, int to) 
    {
        lock (_renderLock)
        {
            for (int i = from; i < to; i++)
                _ignoreLines[i] = new();

            if (_linesRendered > 0)
            {
                for (int i = _linesRendered - 1; i >= 0; i--)
                {
                    if (_ignoreLines.ContainsKey(i))
                    {
                        // Clear Line
                        Con.EditingControlFunctions.EraseInLine(1);
                    }
                    Con.Cursor.Up(1);
                }
                Con.Cursor.Set(_linesRendered, 0);
            }
        }
    }

    private void clearIgnoredLines() 
    {
        _ignoreLines.Clear();
    }

    private void insertTop(IEnumerable<string> lines, int topBoundary, int bottomBoundary) 
    {
        lock (_renderLock)
        {
            var lns = lines.ToList();
            Con.Scroll.Set(topBoundary, bottomBoundary);
            Con.Cursor.Set(topBoundary, 0);
            Con.Scroll.Down(lns.Count);
            Con.Send(string.Join("\r\n", lns));
            Con.Scroll.Set(0, Height);
            Con.Cursor.Set(_linesRendered, 0);
        }
    }

    private void insertBottom(IEnumerable<string> lines, int topBoundary, int bottomBoundary) 
    {
        lock (_renderLock)
        {
            var lns = lines.ToList();
            Con.Scroll.Set(topBoundary, bottomBoundary);
            Con.Cursor.Set(bottomBoundary, 0);
            Con.Send("\r\n" + string.Join("\r\n", lns));
            Con.Scroll.Set(0, Height);
            Con.Cursor.Set(_linesRendered, 0);
        }
    }

    internal void handleMessages(Msg msg) 
    { 
        switch(msg)
        {
            case RepaintMsg _:
                Repaint();
                break;
            case WindowSizeMsg wsm:
                Width = wsm.Width;
                Height = wsm.Height;
                Repaint();
                break;
            case ClearScrollAreaMsg _:
                clearIgnoredLines();
                Repaint();
                break;
            case SyncScrollAreaMsg ssam:
                // Re-render scrolling area
                clearIgnoredLines();
                setIgnoredLines(ssam.topBoundary, ssam.bottomBoundary);
                insertTop(ssam.lines, ssam.topBoundary, ssam.bottomBoundary);

                // Force non-scrolling stuff to repaint in this update cycle
                Repaint();
                break;
            case ScrollUpMsg scrollMsg:
                insertTop(scrollMsg.Lines, scrollMsg.TopBoundary, scrollMsg.BottomBoundary);
                break;
            case ScrollDownMsg scrollMsg:
                insertBottom(scrollMsg.Lines, scrollMsg.TopBoundary, scrollMsg.BottomBoundary);
                break;
            case PrintLineMsg printMsg:
                if (!AltScreen)
                {
                    var lines = printMsg.MessageBody.Split();
                    foreach (var line in lines)
                        _queuedMessageLines.Append(line);
                }
                break;
        }
    }

    public Cmd PrintLn(IEnumerable<Msg> args)
    {
        return new PrintLineMsg(
                string.Join("", args.Select(a => a.ToString()))
            ).ToCmd();
    }
}

