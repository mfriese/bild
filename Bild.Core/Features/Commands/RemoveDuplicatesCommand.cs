using Bild.Core.Features.Files;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Files;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

internal class RemoveDuplicatesCommand : Command<Cli>
{
    public static string Name => "Find duplicate JPG images";

    public override int Execute(CommandContext context, Cli _)
    {
        AnsiConsole.MarkupLine("Select the folder to search for duplicate JPG images.");
        PathSelectorInteractor pathSelector = new();
        var directory = pathSelector.Perform();

        if (!directory.Exists)
            return 1;

        var files = FindJpegFiles(directory).ToList();
        AnsiConsole.MarkupLine($"Found [yellow]{files.Count}[/] JPG/JPEG files.");

        if (files.Count < 2 || !AnsiConsole.Prompt(new ConfirmationPrompt("Compare these files?")))
            return 0;

        FindDuplicateImagesInteractor findDuplicates = new();
        var duplicates = findDuplicates.Perform(files);
        AnsiConsole.MarkupLine($"Found [yellow]{duplicates.Count}[/] possible duplicates.");

        ShowImageComparisonInteractor showComparison = new();
        GetFileHashInteractor getFileHash = new();
        CompareImagesInteractor compareImages = new();
        var deletedCount = 0;

        foreach (var duplicate in duplicates)
        {
            if (!duplicate.FileToKeep.Exists || !duplicate.FileToDelete.Exists)
                continue;

            AnsiConsole.MarkupLine(duplicate.IsExactMatch
                ? "[green]Identical files[/]"
                : $"[yellow]Visually similar files ({duplicate.Similarity:F2} %)[/]");
            AnsiConsole.MarkupLine($"Keep: {Markup.Escape(duplicate.FileToKeep.ToString())}");
            AnsiConsole.MarkupLine($"Delete: {Markup.Escape(duplicate.FileToDelete.ToString())}");
            showComparison.Perform(duplicate.FileToKeep, duplicate.FileToDelete);

            if (!AnsiConsole.Prompt(new ConfirmationPrompt(
                    $"Delete [red]{Markup.Escape(Path.GetFileName(duplicate.FileToDelete.ToString()))}[/]?")))
                continue;

            if (!AreStillDuplicates(duplicate, getFileHash, compareImages))
            {
                AnsiConsole.MarkupLine("[yellow]Files changed since the scan; nothing was deleted.[/]");
                continue;
            }

            try
            {
                duplicate.FileToDelete.Delete();
                deletedCount++;
                AnsiConsole.MarkupLine("[green]File deleted.[/]");
            }
            catch (Exception exception)
            {
                AnsiConsole.MarkupLine($"[red]Could not delete file:[/] {Markup.Escape(exception.Message)}");
            }
        }

        AnsiConsole.MarkupLine($"Deleted [yellow]{deletedCount}[/] file(s).");
        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }

    private static IEnumerable<MediaFile> FindJpegFiles(MediaDir directory)
    {
        var options = new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true };
        return Directory.EnumerateFiles(directory.ToString(), "*", options)
            .Where(path => string.Equals(Path.GetExtension(path), ".jpg", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(Path.GetExtension(path), ".jpeg", StringComparison.OrdinalIgnoreCase))
            .Select(path => new MediaFile(path));
    }

    private static bool AreStillDuplicates(
        DuplicateImagePair duplicate,
        GetFileHashInteractor getFileHash,
        CompareImagesInteractor compareImages)
    {
        try
        {
            var keptHash = getFileHash.Perform(duplicate.FileToKeep);
            var deletedHash = getFileHash.Perform(duplicate.FileToDelete);

            if (keptHash != duplicate.FileToKeepHash || deletedHash != duplicate.FileToDeleteHash)
                return false;

            return duplicate.IsExactMatch
                ? keptHash == deletedHash
                : compareImages.Perform(duplicate.FileToKeep, duplicate.FileToDelete) >= FindDuplicateImagesInteractor.SimilarityThreshold;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
