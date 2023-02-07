namespace RegistrySearcher.Helpers;

public class ConsoleInteractionHelper
{
    public static string RequestInput(string message, ConsoleColor color)
    {
        SetInputColorAndPrintText(message, color);
        var input = Console.ReadLine();
        Console.ResetColor();
        return input;
    }

    public static bool RequestKeystroke(string message, ConsoleKey targetKey, ConsoleColor color)
    {
        SetInputColorAndPrintText(message, color);
        var pressed = Console.ReadKey();
        Console.ResetColor();
        Console.WriteLine();
        return pressed.Key == targetKey;
    }

    public static void PrintColoredLine(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void SetInputColorAndPrintText(string message, ConsoleColor printColor,
        ConsoleColor inputColor = ConsoleColor.White)
    {
        Console.ForegroundColor = printColor;
        Console.Write(message);
        Console.ForegroundColor = inputColor;
    }
}