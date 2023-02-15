namespace RegistrySearcher.Helpers;

public static class ConsoleInteractionHelper
{
    /// <summary>
    ///     Request input from the user and returns the input as a string.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    /// <param name="messageColor">The color to use for the message.</param>
    /// <returns>The user's input as a string.</returns>
    public static string RequestInput(string message, ConsoleColor messageColor)
    {
        SetInputColorAndPrintText(message, messageColor);
        var input = Console.ReadLine();
        Console.ResetColor();
        return input;
    }

    /// <summary>
    ///     Request a keystroke from the user and returns true if the pressed key matches the target key.
    /// </summary>
    /// <param name="message">The message to display to the user.</param>
    /// <param name="targetKey">The key to match against the pressed key.</param>
    /// <param name="messageColor">The color to use for the message.</param>
    /// <returns>True if the pressed key matches the target key, false otherwise.</returns>
    public static bool RequestKeystroke(string message, ConsoleKey targetKey, ConsoleColor messageColor)
    {
        SetInputColorAndPrintText(message, messageColor);
        var pressed = Console.ReadKey();
        Console.ResetColor();
        Console.WriteLine();
        return pressed.Key == targetKey;
    }

    /// <summary>
    ///     Prints a message to the console with the specified color.
    /// </summary>
    /// <param name="message">The message to print.</param>
    /// <param name="messageColor">The color to use for the message.</param>
    public static void PrintColoredLine(string message, ConsoleColor messageColor)
    {
        Console.ForegroundColor = messageColor;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    ///     Sets the color for the message, prints it to the console, and sets the color for user input.
    /// </summary>
    /// <param name="message">The message to print.</param>
    /// <param name="messageColor">The color to use for the message.</param>
    /// <param name="inputColor">The color to use for user input. Defaults to white.</param>
    private static void SetInputColorAndPrintText(string message, ConsoleColor messageColor,
        ConsoleColor inputColor = ConsoleColor.White)
    {
        Console.ForegroundColor = messageColor;
        Console.Write(message);
        Console.ForegroundColor = inputColor;
    }
}