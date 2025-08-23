using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Settings;
using Bild.Core.Interactors.UI;
using CSharpFunctionalExtensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;
internal class NewImportCommand : Command<NewImportSettings>
{
    public static string Name => "Copy from SOURCE to LIBRARY";

    public override int Execute(CommandContext context, NewImportSettings settings)
    {
        AnsiConsole.MarkupLine("Will copy all files from source to target folder.");
        AnsiConsole.MarkupLine("Please select a [red]source folder[/] first!");
        AnsiConsole.MarkupLine("");

        PathSelectorInteractor pathSelector = new();
        var sourcePath = pathSelector.Perform();

        if (string.IsNullOrEmpty(sourcePath))
            return 1;

        GetLibraryPathInteractor getLibraryPath = new();
        var targetPath = getLibraryPath.Perform(settings);

        var tree = new Tree($"[yellow]Import settings[/]");
        tree.AddNode($"Source: [bold]{sourcePath}[/]");
        tree.AddNode($"Target: [bold]{targetPath}[/]");
        AnsiConsole.Write(tree);
        AnsiConsole.MarkupLine("");

        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"Continue?")))
            return 1;

        var scannedFiles = Finder.
            FindFiles(sourcePath).
            ToList();

        var acceptedfiles = scannedFiles.
            Where(ff => ff.IsAccepted).
            ToList();

        scannedFiles.
            RemoveAll(acceptedfiles.Contains);

        foreach (var file in scannedFiles)
        {
            AnsiConsole.MarkupLine($"Removed [yellow]{file.AbsolutePath}[/].");
        }

        AnsiConsole.MarkupLine($"Working with [yellow]{acceptedfiles.Count}[/] files.");
        AnsiConsole.MarkupLine("");

        int counter = 0;

        foreach (var file in acceptedfiles)
        {
            var result = targetPath.Insert(file);

            var resultTree = new Tree(file.AbsolutePath);

            if (result.IsSuccess)
            {
                resultTree.AddNode($"{result.Value}");
            }
            else
            {
                resultTree.AddNode($"{result.Error}");
            }

            resultTree.AddNode($"File {++counter} of {acceptedfiles.Count}.");

            AnsiConsole.Write(resultTree);
            AnsiConsole.MarkupLine("");
        }

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}
