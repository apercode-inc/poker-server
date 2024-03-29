namespace server.Code.GlobalUtils;

public static class Logger
{
    public static void Debug(string text, ConsoleColor color = ConsoleColor.Cyan, bool isSend = false)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();

        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Debug);
        }
    }
    
    public static void Error(string text, bool isSend = false)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();

        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Error);  
        }
    }

    public static void LogWarning(string text, bool isSend = false)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ResetColor();

        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Warning);  
        }
    }
}