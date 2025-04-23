using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class ImportCommand : Command<ImportSettings>
{
    public static string Name => "Import files into photos library";

    public override int Execute(CommandContext context, ImportSettings settings)
    {
        AnsiConsole.MarkupLine($"Current setting for import. I will MOVE all " +
            "files from source to target folder. They will be remove from the " +
            "source folder and added to the target folder. Please select a " +
            "[red]source folder[/] first!\r\n");

        PathSelectorInteractor pathSelector = new();
        var sourcePath = pathSelector.Perform();

        var pp = new ConfirmationPrompt($"Import media from [red]{sourcePath}[/] " +
            $"to target folder [red]{settings.PhotosDir}[/]");

        if (!AnsiConsole.Prompt(pp))
            return 1;

        var files = Finder.FindFiles(sourcePath);

        foreach (var file in files.Where(ff => ff.IsAccepted))
        {
            file.Move(settings.PhotosDir, "yyyy-MM-dd_hh-mm-ss");

            AnsiConsole.MarkupLine($"[green]File {file.Filename} moved to {settings.PhotosDir}[/]");
        }

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}
