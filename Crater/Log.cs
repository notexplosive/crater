﻿namespace Crater;

public static class Log
{
    public enum Severity
    {
        Info,
        Warning,
        Error
    }

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
        Console.WriteLine($"{Log.SeverityCharacter(severity)}💬 {string.Join(" ", objects)}");
    }

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

    public static void Error(string prefix,string message)
    {
        Console.WriteLine($"{prefix}{Log.SeverityCharacter(Severity.Error)} {message}");
    }
}
