using System.ComponentModel;
using System.Diagnostics;

namespace Crater.ExternalPrograms;

public class ProgramOutput
{
    public readonly bool WasSuccessful;

    public ProgramOutput(bool wasSuccessful)
    {
        WasSuccessful = wasSuccessful;
    }
}

public enum ProgramOutputLevel
{
    AllowProgramToEmitToConsole,
    SuppressProgramFromEmittingToConsole
}

public class ExternalProgram
{
    private readonly string _runPath;

    public ExternalProgram(string runPath)
    {
        _runPath = runPath;
    }

    public ProgramOutput RunWithArgs(ProgramOutputLevel outputLevel, params string[] argumentList)
    {
        var workingDirectory = Directory.GetCurrentDirectory();

        if (outputLevel == ProgramOutputLevel.AllowProgramToEmitToConsole)
        {
            Log.Info("💻", _runPath + (argumentList.Length > 0 ? " " : "") + string.Join(" ", argumentList));
        }

        var wasSuccessful = true;
        using (var process = new Process())
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = _runPath;
            process.StartInfo.UseShellExecute = false;

            if (outputLevel == ProgramOutputLevel.SuppressProgramFromEmittingToConsole)
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
            }

            foreach (var argument in argumentList)
            {
                process.StartInfo.ArgumentList.Add(argument);
            }

            try
            {
                process.Start();

                if (outputLevel == ProgramOutputLevel.SuppressProgramFromEmittingToConsole)
                {
                    // Flush the buffers
                    process.StandardOutput.ReadToEnd();
                    process.StandardError.ReadToEnd();
                }

                process.WaitForExit();
            }
            catch (Win32Exception)
            {
                wasSuccessful = false;
            }
        }

        return new ProgramOutput(wasSuccessful);
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.Write(e.Data);
    }
}
