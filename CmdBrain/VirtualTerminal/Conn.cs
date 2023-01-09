namespace No8.CmdBrain.VirtualTerminal;

public static partial class Con
{
    public static event EventHandler<ConsoleKeyInfo>? KeyAvailable;
    public static event EventHandler<string>? SequenceAvailable;

    public static bool MonitoringInput
    {
        get => _monitoringInput;
    }

    public static void Stop()
    {
        Trace.WriteLine("Con.Stopping");
        _exitEvent.Reset();
        _monitoringInput = false;

        // Wait for Input monitor thread to exit
        _exitEvent.Wait(500);
        Trace.WriteLine("Con.Stopped");
    }

    private static bool _monitoringInput;
    private static readonly ManualResetEventSlim _exitEvent = new(false);

    static Con()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding =  Encoding.UTF8;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelEventHandler);
        _monitoringInput                 =  true;
        
        Task.Run(MonitorInput);
    }

    public static void Start()
    {
        if (MonitoringInput) return;
        _monitoringInput = true;

        Trace.WriteLine("Con.Start");
        Task.Run(MonitorInput);
    }

    private static void ConsoleCancelEventHandler(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;    // I'll take it from here
        Stop();
    }

    private static async Task MonitorInput()
    {
        while (_monitoringInput)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo? key;
                try
                {
                    key = Console.ReadKey(true);
                    if (key != null)
                        AddKey(key.Value);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Con.Exception reading key: " + e);
                    break;
                }
            }
            else
                await Task.Delay(50);   // So we don't overload the cpu
        }

        Trace.WriteLine("Exiting Monitoring");
        _exitEvent.Set();
    }

    /// Escape Sequence
    ///     ESC                 0x1b
    ///     Intermediate        0x20..0x2F  <SP>!"#$%&'()*+,-./
    ///     Final               0x30..0x7E  0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
    /// Control Sequence
    ///     CSI                 0x1b [
    ///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
    ///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
    ///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
    /// Device Control Strings
    ///     DCS                 0x1b P
    ///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
    ///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
    ///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
    ///     Data String         *****
    ///     String Term         0x1b \
    /// Parameters are always unsigned decimal numbers, separated by ;
    /// CAN (0x18) Cancel can cancel a sequence. Indicates an error
    /// SUB (0x1a) Cancel a sequence in progress
    private static void AddKey(ConsoleKeyInfo key)
    {
        Trace.WriteLine($"IN: {key.Key}, {key.KeyChar}"
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? ":Alt" : "")
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Control) ? ":Ctrl" : "")
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? ":Shift" : ""));

        var ch = key.KeyChar;
        if (ch == (char)ControlChar.ESC && !EscapeSequence)
        {
            EscapeSequence = true;
            Sequence.Clear();
            Sequence.Append(ch);
            return;
        }

        if (EscapeSequence)
        {
            Sequence.Append(ch);
            if (Sequence.Length == 2)
            {
                switch (ch)
                {
                case 'I':   // In Focus
                    RaiseSequenceAvailable();
                    break;
                case 'O':   // Out of focus
                    RaiseSequenceAvailable();
                    break;
                
                case 'P':  // DCS   Device control string
                    DeviceControlString = true;
                    break;
                case 'X': // SOS  Start of String
                    StartOfString = true;
                    break;
                case '[': // CSI  Control Sequence Introducer
                    ControlSequenceIntroducer = true;
                    break;
                case '\\': // ST   String Terminator
                    StringTerminator();
                    break;
                case ']': // OSC  Operating System Command
                    OperatingSystemCommand = true;
                    break;
                case '^': // PM  Privacy Message
                    PrivacyMessage = true;
                    break;
                case '_': // APC    Application Program Command
                    ApplicationProgramCommand = true;
                    break;
                }
            }
            else
            {
                if (IsFinal(ch))
                {
                    RaiseSequenceAvailable();
                }
            }

            return;
        }

        RaiseKeyAvailable(key);
    }

    private static string CloseSequence()
    {
        string value = null!;
        lock (_sequenceLock)
        {
            value = Sequence.ToString();
            Sequence.Clear();
            EscapeSequence            = false;
            ApplicationProgramCommand = false;
            ControlSequenceIntroducer = false;
            DeviceControlString       = false;
            OperatingSystemCommand    = false;
            PrivacyMessage            = false;
            StartOfString             = false;
        }

        return value;
    }

    private static readonly object _sequenceLock = new();
    private static bool EscapeSequence;
    private static bool ApplicationProgramCommand; // APC
    private static bool ControlSequenceIntroducer; // CSI
    private static bool DeviceControlString;       // DCS
    private static bool StartOfString;             // SOS
    private static bool OperatingSystemCommand;    // OSC
    private static bool PrivacyMessage;            // PM
    private static readonly StringBuilder Sequence = new();

    internal static bool IsIntermediate(this char ch) => ch >= 0x20 && ch <= 0x2F;
    internal static bool IsParameter(this char ch) => ch >= 0x30 && ch <= 0x3F;
    internal static bool IsFinal(this char ch) => ch >= 0x40 && ch <= 0x7E;

    private static void StringTerminator()
    {
        var value = CloseSequence();
    }

    private static void RaiseKeyAvailable(ConsoleKeyInfo e) => KeyAvailable?.Invoke(null, e);

    private static void RaiseSequenceAvailable()
    {
        var value = CloseSequence();
        SequenceAvailable?.Invoke(null, value);
    }

    public static void Send(string value)
    {
        //if (!_monitoringInput)
        //    return;
        
        #if DEBUG
        var sb = new StringBuilder("Send: ");
        foreach (var ch in value)
        {
            if (ch < ' ')
                sb.Append($"<{(ControlChar)ch}>");
            else
                sb.Append(ch);
        }
        Trace.WriteLine(sb.ToString());
        #endif
            
        Console.Write(value);
    }

}
