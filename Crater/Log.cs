namespace Crater;

public static class Log
{
    public enum Severity
    {
        Info,
        Warning,
        Error
    }

    public const string CorePrefix = "🌙";

    public static void Info(string prefix, string message)
    {
        Console.WriteLine($"{Log.SeverityCharacter(Severity.Info)}{prefix} {message}");
    }

    public static void Warning(string prefix, string message)
    {
        Console.WriteLine($"{Log.SeverityCharacter(Severity.Warning)}{prefix} {message}");
    }

    public static void FromLua(Severity severity, params object[] objects)
    {
        Console.WriteLine($"{Log.SeverityCharacter(severity)}{LuaPrefix} {string.Join(" ", objects)}");
    }

    private const string LuaPrefix = "🐲";
    
    private static string SeverityCharacter(Severity severity)
    {
        switch (severity)
        {
            case Severity.Info:
                return "🔵";
            case Severity.Warning:
                return "🔶";
            case Severity.Error:
                return "🟥";
            default:
                return "❔";
        }
    }

    public static void Error(string prefix, string message)
    {
        Console.WriteLine($"{Log.SeverityCharacter(Severity.Error)}{prefix} {message}");
    }
}
