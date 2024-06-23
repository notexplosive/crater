namespace Crater;

public class ProgramOutput
{
    public readonly bool WasSuccessful;

    public ProgramOutput(bool wasSuccessful, string[] output)
    {
        WasSuccessful = wasSuccessful;
        Output = output;
    }

    public string[] Output { get; }
}
