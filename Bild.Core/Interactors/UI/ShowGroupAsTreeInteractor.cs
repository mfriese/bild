using Spectre.Console;

namespace Bild.Core.Interactors.UI;

public class ShowGroupAsTreeInteractor
{
    public void Perform(IGrouping<string, string> grouping)
    {
        var tree = new Tree($"[yellow]Root: {grouping.Key}[/]");

        foreach (var item in grouping)
            tree.AddNode($"[bold]{item}[/]");

        AnsiConsole.Write(tree);
    }
}
