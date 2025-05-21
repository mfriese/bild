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

        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Use your photo library " +
            $"folder [red]{selectedPath}[/]?")))
        {
            PathSelectorInteractor pathSelector = new();
            var path = pathSelector.Perform(selectedPath.AbsolutePath);

            selectedPath = string.IsNullOrEmpty(path) ?
                MediaDir.Empty :
                new MediaDir(path);
        }

        return selectedPath;
    }
}
