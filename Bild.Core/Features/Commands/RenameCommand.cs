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

        if (string.IsNullOrEmpty(selectedDir))
            return 0;

        var files = Finder.FindFiles(selectedDir);

        var table = new Table()
            .Border(TableBorder.Ascii)
            .BorderColor(Color.White)
            .Expand()
            .AddColumn("[cyan]Filename[/]")
            .AddColumn("[cyan]Filetype[/]")
            .AddColumn("[grey]EXIF Created at[/]")
            .AddColumn("[grey]EXIF File suffix[/]");

        GetProgressInteractor getProgress = new();
        var exifProgress = getProgress.Perform();

        AnsiConsole.MarkupLine("Now getting Meta Data for each file ...");

        exifProgress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Count()} files[/]", maxValue: files.Count());

            files.ToList().ForEach(ff =>
            {
                table.AddRow(
                    Markup.Escape(ff.Filename),
                    ff.ExifFileType?.ToString() ?? "N/A",
                    ff.ExifCreationDate?.ToString("yyyy-MM-dd hh:mm:ss") ?? "N/A",
                    ff.ExifFileNameExtension ?? "N/A");

                task.Increment(1);
            });
        });

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine("Images and Videos that have a creation date will " +
            "be renamed. Files that do not have such a date will not be changed!\r\n");

        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Proceed to rename files?")))
            return 0;

        var renameProgress = getProgress.Perform();

        exifProgress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Count()} files[/]", maxValue: files.Count());

            files.Where(ff => ff.IsAccepted).ToList().ForEach(ff =>
            {
                ff.RenameToDateTemplate("yyyy-MM-dd_hh-mm-ss");

                task.Increment(1);
            });
        });

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}
