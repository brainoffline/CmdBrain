namespace No8.CmdBrain;

public delegate Msg ExecCallback(Exception? error);

public interface ExecCommand
{
    public Task Run();
    public void SetStdIn(TextReader input);
    public void SetStdOut(TextWriter output);
    public void SetStdErr(TextWriter errOutput);
}

public record ExecMsg(
    ExecCommand Cmd,
    ExecCallback? Callback
    ) : Msg
{ }



public static class Exec
{
    public static Cmd Process(string filename, params string[] args)
    {
        var startInfo = new ProcessStartInfo(filename);
        foreach (var arg in args)
            startInfo.ArgumentList.Add(arg);
        return Process(startInfo);
    }

    public static Cmd Process(ProcessStartInfo startInfo, ExecCallback? callback = null)
    {
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;

        var process = new Process
        {
            StartInfo = startInfo
        };

        var execCommand = new ProcessExecCommand(process);
        return new ExecMsg(execCommand, callback).ToCmd();
    }
}

internal class ProcessExecCommand : ExecCommand
{
    private Process _process;
    private TextReader? _input;

    public ProcessExecCommand(Process process)
    {
        _process = process;
    }

    public async Task Run()
    {
        _process.Start();
        if (_process.StartInfo.RedirectStandardError)
            _process.BeginErrorReadLine();
        if (_process.StartInfo.RedirectStandardOutput)
            _process.BeginOutputReadLine();
        if (_input != null)
        {
            while (!_process.HasExited)
            {
                var line = await _input.ReadLineAsync();
                _process.StandardInput.WriteLine(line);
            }
        }
        await _process.WaitForExitAsync();
    }


    public void SetStdIn(TextReader input)
    {
        _process.StartInfo.RedirectStandardInput = true;
        _input = input;
    }

    public void SetStdOut(TextWriter output)
    {
        _process.StartInfo.RedirectStandardOutput = true;
        _process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                output.WriteLine(e.Data);
        };
    }

    public void SetStdErr(TextWriter errOutput)
    {
        _process.StartInfo.RedirectStandardError = true;
        _process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                errOutput.WriteLine(e.Data);
        };
    }

}

