using Spectre.Console;

namespace Application;

internal static class ConsoleHelper
{
    public static void WaitForTask(Action action, string waitMessage)
        => AnsiConsole.Status().Start("Thinking...", ctx =>
        {
                // Set the status and spinner
            ctx.Status(waitMessage);
            ctx.Spinner(Spinner.Known.Star);
            ctx.SpinnerStyle(new Style(Color.Green));

                // Execute the action
            action();
        });
}
