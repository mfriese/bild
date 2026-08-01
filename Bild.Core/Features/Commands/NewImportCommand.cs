using Bild.Core.Features.Files;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Settings;
using Bild.Core.Interactors.UI;
using CSharpFunctionalExtensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

internal class NewImportCommand : Command<Cli>
{
    public static string Name => "Copy from SOURCE to LIBRARY";

    public override int Execute(CommandContext context, Cli _)
    {
        AnsiConsole.MarkupLine("Will copy all files from source to target folder.");
        AnsiConsole.MarkupLine("Please select a [red]source folder[/] first!");
        AnsiConsole.MarkupLine("");

        PathSelectorInteractor pathSelector = new();
        var sourceDirectory = pathSelector.Perform();

        if (!sourceDirectory.Exists)
            return 1;

        LoadConfigurationInteractor loadConfiguration = new();
        var configuration = loadConfiguration.Perform();

        var targetDirectory = new MediaDir(configuration.PhotosDir);
        
        var tree = new Tree($"[yellow]Import settings[/]");
        tree.AddNode($"Source: [bold]{sourceDirectory}[/]");
        tree.AddNode($"Target: [bold]{targetDirectory}[/]");
        AnsiConsole.Write(tree);
        AnsiConsole.MarkupLine("");

        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"Continue?")))
            return 1;

        AnsiConsole.MarkupLine("");
        AnsiConsole.Markup(Markup.Escape("Collecting ["));
        var scannedFiles = sourceDirectory.FindFilesRecursive().ToList();
        AnsiConsole.MarkupLine(Markup.Escape("]"));

        var acceptedFiles = new List<MediaFile>();

        GetProgressInteractor getProgress = new();
        var progressIndicator = getProgress.Perform();

        progressIndicator.Start(ctx =>
        {
            var task = ctx.AddTask(
                $"[green]Scanning {scannedFiles.Count()} files[/]",
                maxValue: scannedFiles.Count());

            acceptedFiles.AddRange(scannedFiles.Where(ff =>
            {
                task.Increment(1);
                return ff.IsAccepted;
            }));
        });

        scannedFiles.RemoveAll(acceptedFiles.Contains);

        foreach (var scannedFile in scannedFiles)
        {
            AnsiConsole.MarkupLine($"Removed [yellow]{scannedFile}[/].");
        }

        AnsiConsole.MarkupLine($"Working with [yellow]{acceptedFiles.Count}[/] files.");
        AnsiConsole.MarkupLine("");

        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"Continue?")))
            return 1;

        int counter = 0;
        int delete = 1; // 0 = never, 1 = no, 2 = yes, 3 = always

        MediaDir targetDir = new MediaDir(configuration.PhotosDir);
        
        try
        {
            foreach (var acceptedFile in acceptedFiles)
            {
                var result = CopyFile(acceptedFile, targetDir);

                var resultTree = new Tree(acceptedFile.ToString());

                resultTree.AddNode(result.IsSuccess ? $"{result.Value}" : $"{result.Error}");

                resultTree.AddNode($"File {++counter} of {acceptedFiles.Count}.");

                AnsiConsole.Write(resultTree);
                AnsiConsole.MarkupLine("");

                if (!result.IsSuccess)
                {
                    continue;
                }

                if (delete is 1 or 2)
                {
                    delete = AnsiConsole.Prompt(
                        new TextPrompt<int>("Delete? (0)never, (1)no, (2)yes, (3)always")
                            .AddChoice(0)
                            .AddChoice(1)
                            .AddChoice(2)
                            .AddChoice(3)
                            .DefaultValue(delete));
                }

                switch (delete)
                {
                    case 0 or 1:
                        continue;
                    case 2 or 3:
                        acceptedFile.Delete();
                        AnsiConsole.MarkupLine($"Deleted [yellow]{acceptedFile}[/].");
                        AnsiConsole.MarkupLine($"");
                        break;
                }
            }
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[red]App has crashed![/].");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            WaitKeyPressInteractor waitCrashKeyPress = new();
            return waitCrashKeyPress.Perform(1);
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