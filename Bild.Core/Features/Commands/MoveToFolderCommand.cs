using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Settings;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class MoveToFolderCommand : Command<MoveToFolderSettings>
{
    public static string Name => "Insert imports into library";

    public override int Execute(CommandContext context, MoveToFolderSettings settings)
    {
        GetImportPathInteractor getImportPath = new();
        var sourcePath = getImportPath.Perform(settings);

        GetLibraryPathInteractor getLibraryPath = new();
        var targetPath = getLibraryPath.Perform(settings);

        var pp = new ConfirmationPrompt($"Move media from [red]{sourcePath}[/] to target " +
            $"folder [red]{targetPath}[/]? Continue?");

        if (!AnsiConsole.Prompt(pp))
            return 1;

        var files = Finder.FindFiles(sourcePath?.AbsolutePath);

        foreach (var file in files)
        {
            if (!file.IsAccepted)
            {
                AnsiConsole.MarkupLine($"[red]{file.AbsolutePath}[/] -> not accepted.");

                continue;
            }

            var creationDate = file.ExifCreationDate;

            var year = creationDate?.ToString("yyyy") ?? string.Empty;
            var month = creationDate?.ToString("MM") ?? string.Empty;

            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
            {
                AnsiConsole.MarkupLine($"[red]{file.AbsolutePath}[/] -> no exif data.");

                continue;
            }

            var targetRootDir = targetPath?.AbsolutePath ?? string.Empty;
            var targetYearDir = Path.Combine(targetRootDir, year);
            var targetDir = Path.Combine(targetYearDir, month);

            if (!Directory.Exists(targetYearDir))
                Directory.CreateDirectory(targetYearDir);

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            AnsiConsole.Markup($"Moving [cyan]{file.AbsolutePath}[/] to [cyan]{targetDir}[/] ... ");

            if (file.MoveTo(new MediaDir(targetDir)) is not null)
            {
                AnsiConsole.MarkupLine("[green]success[/]!");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]failed[/]! (file untouched)");
            }
        }

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}