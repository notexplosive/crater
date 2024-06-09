using System.ComponentModel;
using System.Diagnostics;

namespace Crater.ExternalPrograms;

public class ProgramOutput
{
    public readonly bool WasSuccessful;
    public string[] Output { get; }

    public ProgramOutput(bool wasSuccessful, string[] output)
    {
        WasSuccessful = wasSuccessful;
        Output = output;
    }
}

public class ExternalProgram
{
    private readonly string _runPath;

    public ExternalProgram(string runPath)
    {
        _runPath = runPath;
    }

    public ProgramOutput RunWithArgs(params string[] argumentList)
    {
        var workingDirectory = Directory.GetCurrentDirectory();

            Log.Info("💻", _runPath + (argumentList.Length > 0 ? " " : "") + string.Join(" ", argumentList));

        var wasSuccessful = true;
        var totalOutput = new List<string>();
        using (var process = new Process())
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = _runPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            
            process.OutputDataReceived += (sender, found) =>
            {
                if (!string.IsNullOrEmpty(found.Data))
                {
                    Console.WriteLine(found.Data);
                    totalOutput.Add(found.Data);
                }
            };

            foreach (var argument in argumentList)
            {
                process.StartInfo.ArgumentList.Add(argument);
            }
            try
            {
                process.Start();
                
                process.BeginOutputReadLine();

                process.WaitForExit();
            }
            catch (Win32Exception)
            {
                wasSuccessful = false;
            }
        }

        return new ProgramOutput(wasSuccessful, totalOutput.ToArray());
    }
}
