using Spectre.Console;

namespace Bild.Core.Interactors.UI;

public class WaitKeyPressInteractor
{
    public T Perform<T>(T returnValue)
    {
        AnsiConsole.MarkupLine("Press [green]any key[/] to continue ...");
        Console.ReadKey(true);
        return returnValue;
    }
}
