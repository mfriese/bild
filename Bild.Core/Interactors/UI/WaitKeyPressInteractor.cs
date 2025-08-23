using Spectre.Console;

namespace Bild.Core.Interactors.UI;

public class WaitKeyPressInteractor
{
    public T Perform<T>(T returnValue)
    {
        AnsiConsole.MarkupLine("Press [green]ESC[/] to continue ...");

        ConsoleKey key;
        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Escape);

        return returnValue;
    }
}
