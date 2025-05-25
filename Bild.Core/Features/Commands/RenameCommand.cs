using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class RenameCommand : Command<RenameSettings>
{
    public static string Name => "Rename files to Exif Date";

    public override int Execute(CommandContext context, RenameSettings settings)
    {
        AnsiConsole.MarkupLine("You have chosen to [red]rename[/] files. You " +
            "can cancel at any point along the way but once files are renamed " +
            "the orignal name is gone! No files will be deleted!\r\n");

        ImportPathOrSelectInteractor importPathOrSelect = new();
        var selectedDir = importPathOrSelect.Perform();

        if (string.IsNullOrEmpty(selectedDir?.AbsolutePath))
            return 0;

        var files = Finder.FindFiles(selectedDir);

        AnsiConsole.MarkupLine("Now getting Meta Data for each file ...");

        var previewTable = new Table()
            .Border(TableBorder.Ascii)
            .BorderColor(Color.White)
            .Expand()
            .AddColumn("[cyan]Old Filename[/]")
            .AddColumn("[red]New Filename[/]")
            .AddColumn("[grey]Extension[/]");

        GetProgressInteractor getProgress = new();
        var exifProgress = getProgress.Perform();

        exifProgress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Count()} files[/]", maxValue: files.Count());

            files.ToList().ForEach(ff =>
            {
                task.Increment(1);
                previewTable.AddRow(new[]
                {
                    ff.Filename,
                    GetFilename(ff) ?? "N/A",
                    ff.ExifFileNameExtension ?? ff.Extension,
                });
            });
        });

        AnsiConsole.Write(previewTable);

        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Proceed to rename files?")))
            return 0;

        var renameProgress = getProgress.Perform();

        var renameTable = new Table()
            .Border(TableBorder.Ascii)
            .BorderColor(Color.White)
            .Expand()
            .AddColumn("[cyan]Filename[/]")
            .AddColumn("[red]status[/]");

        renameProgress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Count()} files[/]", maxValue: files.Count());

            var result = files.Select(ff =>
            {
                task.Increment(1);

                var dateFilename = GetFilename(ff);

                if (dateFilename is null)
                {
                    return new[] { ff.Filename, "cannot create filename" };
                }

                var newFilename = $"{dateFilename}.{ff.ExifFileNameExtension ?? ff.Extension}";
                var newFilePath = Path.Combine(ff.Dir.AbsolutePath, newFilename);

                if (newFilePath == ff.AbsolutePath)
                {
                    return [ff.Filename, "already has correct name"];
                }

                for(int ii = 1; File.Exists(newFilePath); ++ii)
                {
                    newFilename = $"{dateFilename}_{ii}.{ff.ExifFileNameExtension ?? ff.Extension}";
                    newFilePath = Path.Combine(ff.Dir.AbsolutePath, newFilename);
                }

                File.Move(ff.AbsolutePath, newFilePath, false);

                return [ff.Filename, $"renamed to {newFilename}"];
            });

            result.ToList().ForEach(rr => renameTable.AddRow(rr));
        });

        AnsiConsole.Write(renameTable);

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }

    private string GetFilename(MediaFile file)
    {
        if (file.ExifCreationDate?.ToString("yyyyMMdd_hhmmss") is string date)
            return "img_" + date;
        return null;
    }
}
