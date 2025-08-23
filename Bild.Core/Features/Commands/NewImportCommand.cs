using Bild.Core.Features.Files;
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

        var acceptedFiles = new List<MediaFile>();
        
        GetProgressInteractor getProgress = new();
        var progressIndicator = getProgress.Perform();
        
        progressIndicator.Start(ctx => 
        {
            var task = ctx.AddTask(
                $"[green]Scanning {scannedFiles.Count} files[/]",
                maxValue: scannedFiles.Count);

            acceptedFiles.AddRange(scannedFiles.Where(ff =>
            {
                task.Increment(1);
                return ff.IsAccepted;
            }));
        });
        
        scannedFiles.
            RemoveAll(acceptedFiles.Contains);

        foreach (var file in scannedFiles)
        {
            AnsiConsole.MarkupLine($"Removed [yellow]{file.AbsolutePath}[/].");
        }

        AnsiConsole.MarkupLine($"Working with [yellow]{acceptedFiles.Count}[/] files.");
        AnsiConsole.MarkupLine("");

        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"Continue?")))
            return 1;
        
        int counter = 0;

        foreach (var file in acceptedFiles)
        {
            var result = CopyFile(file, targetPath);

            var resultTree = new Tree(file.AbsolutePath);

            if (result.IsSuccess)
            {
                resultTree.AddNode($"{result.Value}");
            }
            else
            {
                resultTree.AddNode($"{result.Error}");
            }

            resultTree.AddNode($"File {++counter} of {acceptedFiles.Count}.");

            AnsiConsole.Write(resultTree);
            AnsiConsole.MarkupLine("");
        }

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }

    private static Result<string> CopyFile(MediaFile file, MediaDir target)
    {
        var creationDate = file.ExifCreationDate;
        var year = creationDate?.ToString("yyyy") ?? string.Empty;
        var month = creationDate?.ToString("MM") ?? string.Empty;

        if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
        {
            return Result.Failure<string>($"[red]Cannot find EXIF data.[/]");
        }

        var targetSubDir = target.
            GetOrCreateSubdirectory(year).
            GetOrCreateSubdirectory(month);

        return targetSubDir.Insert(file);
    }
}
