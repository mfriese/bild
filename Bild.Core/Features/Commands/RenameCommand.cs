using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Directories;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class RenameCommand : Command<RenameSettings>
{
    public static string Name => "Rename Files to Exif Date";

    public override int Execute(CommandContext context, RenameSettings settings)
    {
        AnsiConsole.MarkupLine("You have chosen to [red]rename[/] files. You " +
            "can cancel at any point along the way but once files are renamed the orignal " +
            "name is gone! No files will be deleted!\r\n");

        DriveSelectorInteractor driveSelector = new();
        var selectedDrive = driveSelector.Perform();

        if (string.IsNullOrEmpty(selectedDrive))
            return CancellationMessage(0);

        DirectorySelectorInteractor directorySelector = new();
        var selectedDir = directorySelector.Perform(selectedDrive);

        if (string.IsNullOrEmpty(selectedDir))
            return CancellationMessage(0);

        var files = Finder.FindFiles(selectedDir);

        var creation = files.Where(ff => ff.ExifCreationDate is not null).ToList();

        var msg = $"Found {files.Count()} files, but only {creation.Count} have EXIF Creation information! Proceed?";

        if (!AnsiConsole.Prompt(new ConfirmationPrompt(msg)))
            return CancellationMessage(0);

        foreach (var file in creation)
        {
            // TODO name collisions

            var newName = file.ExifCreationDate?.ToString("yyyy-MM-dd_hh-mm-ss") + "." + file.ExifFileNameExtension;

            AnsiConsole.MarkupLine($"Renaming {file.Filename} to {newName}");

            var newFile = file.Rename(newName);

            if (newFile is null)
                AnsiConsole.MarkupLine($"[red]Problem[/] when renaming {file.Filename} to {newName}!");
        }

        AnsiConsole.MarkupLine($"[green]Renamed {creation.Count} files![/]");
        AnsiConsole.MarkupLine("Press [green]any key[/] to continue ...");
        Console.ReadKey();

        return 0;
    }

    private int CancellationMessage(int returnValue)
    {
        AnsiConsole.MarkupLine("[red]Operation cancelled![/]");

        return returnValue;
    }
}
