using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Hashing;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class DuplicatesCommand : Command<DuplicatesSettings>, INamedCommand
{
    public string CommandName => "Delete Duplicates";

    public override int Execute(CommandContext context, DuplicatesSettings settings)
    {
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt("You have chosen to delete duplicate files. Continue?"));

        if (!confirmation)
            return CancellationMessage(0);

        DriveSelectorInteractor driveSelector = new();
        var selectedDrive = driveSelector.Perform();

        if (string.IsNullOrEmpty(selectedDrive))
            return CancellationMessage(0);

        DirectorySelectorInteractor directorySelector = new();
        var selectedDir = directorySelector.Perform(selectedDrive);

        if (string.IsNullOrEmpty(selectedDir))
            return CancellationMessage(0);

        GetAllHashesInteractor getAllHashes = new();
        var hashes = getAllHashes.Perform(selectedDir);

        if (!hashes.Any(hh => 1 < hh.Count()))
        {
            AnsiConsole.MarkupLine($"[green]There are NO duplicates in {selectedDir}[/]");

            return 0;
        }

        //foreach (var hash in hashes)
        //{
        //    ShowGroupAsTreeInteractor showGroupAsTree = new();
        //    showGroupAsTree.Perform(hash);
        //}

        return 0;
    }

    private int CancellationMessage(int returnValue)
    {
        AnsiConsole.MarkupLine("[red]Operation cancelled![/]");

        return returnValue;
    }
}
