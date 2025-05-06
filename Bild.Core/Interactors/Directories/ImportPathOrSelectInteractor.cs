using Bild.Core.Features.Files;
using Bild.Core.Interactors.Settings;
using Spectre.Console;

namespace Bild.Core.Interactors.Directories;

public class ImportPathOrSelectInteractor
{
    public MediaDir Perform()
    {
        GetLibraryPathInteractor getLibraryPath = new();
        var selectedPath = getLibraryPath.Perform();

        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Use you photo library " +
            $"folder [red]{selectedPath}[/]?")))
        {
            PathSelectorInteractor pathSelector = new();
            selectedPath = new MediaDir(pathSelector.Perform(selectedPath.AbsolutePath));
        }

        return selectedPath;
    }
}
