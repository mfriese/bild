using Bild.Core.Interactors.Settings;
using Spectre.Console;

namespace Bild.Core.Interactors.Directories;

public class ImportPathOrSelectInteractor
{
    public string Perform()
    {
        GetImportPathInteractor getImportPath = new();
        var selectedPath = getImportPath.Perform();

        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Use you photo libraries " +
            $"import folder [red]{selectedPath}[/]?")))
        {
            PathSelectorInteractor pathSelector = new();
            selectedPath = pathSelector.Perform();
        }

        return selectedPath;
    }
}
