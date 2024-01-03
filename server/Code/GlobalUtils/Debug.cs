namespace server.Code.GlobalUtils;

public static class Debug
{
    public static void LogColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    public static void LogError(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void Log(string text)
    {
        Console.WriteLine(text);
    }
}