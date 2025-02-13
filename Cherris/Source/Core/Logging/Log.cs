namespace Cherris;

using System;
using System.IO;
using System.Runtime.CompilerServices;

public class Log
{
    private readonly static ConsoleColor infoColor = ConsoleColor.DarkGray;
    private readonly static ConsoleColor warningColor = ConsoleColor.Yellow;
    private readonly static ConsoleColor errorColor = ConsoleColor.Red;

    public static void Info(string message, string condition = "ClickServer/ViableClickable",
        [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
    {
        if (LogSettings.Instance.GetLogCondition("Info") && LogSettings.Instance.GetLogCondition(condition))
        {
            Console.ForegroundColor = infoColor;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] {Path.GetFileName(filePath)}:{lineNumber} {message}");
            Console.ResetColor();
        }
    }

    public static void Warning(string message, string condition = "ClickServer/ViableClickable",
        [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
    {
        if (LogSettings.Instance.GetLogCondition("Warning") && LogSettings.Instance.GetLogCondition(condition))
        {
            Console.ForegroundColor = warningColor;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [WARNING] {Path.GetFileName(filePath)}:{lineNumber} {message}");
            Console.ResetColor();
        }
    }

    public static void Error(string message, string condition,
        [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
    {
        if (!LogSettings.Instance.GetLogCondition("Error") || !LogSettings.Instance.GetLogCondition(condition))
        {
            return;
        }

        Error(message, lineNumber, filePath);
    }

    public static void Error(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
    {
        string fullMessage = $"[{DateTime.Now:HH:mm:ss}] [ERROR] {Path.GetFileName(filePath)}:{lineNumber} {message}";

        Console.ForegroundColor = errorColor;
        Console.WriteLine(fullMessage);
        Console.ResetColor();

        File.AppendAllText("Res/Log.txt", Environment.NewLine + fullMessage);
    }
}