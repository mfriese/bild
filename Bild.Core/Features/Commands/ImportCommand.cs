using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Settings;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class ImportCommand : Command<ImportSettings>
{
    public static string Name => "_Move to import folder";

    public override int Execute(CommandContext context, ImportSettings settings)
    {
        AnsiConsole.MarkupLine($"Current setting for import. I will MOVE all files from source " +
            "to target folder. Please select a [red]source folder[/] first!\r\n");

        PathSelectorInteractor pathSelector = new();
        var sourcePath = pathSelector.Perform();

        if (string.IsNullOrEmpty(sourcePath))
            return 0;

        GetImportPathInteractor getImportPath = new();
        var targetPath = getImportPath.Perform(settings);

        var pp = new ConfirmationPrompt($"Recursively import media from [red]{sourcePath}[/] " +
            $"to target folder [red]{targetPath}[/]? Files will be moved. Continue?");

        if (!AnsiConsole.Prompt(pp))
            return 1;

        var files = Finder.FindFiles(sourcePath);

        GetProgressInteractor getProgress = new();
        var progress = getProgress.Perform();

        List<string> copiedFiles = [];
        List<string> skippedFiles = [];

        progress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Count()} files[/]", maxValue: files.Count());

            foreach (var file in files)
            {
                task.Increment(1);

                if (!file.IsAccepted)
                {
                    skippedFiles.Add(file.AbsolutePath);
                    AnsiConsole.MarkupLine($"[red]{file.AbsolutePath}[/] is skipped.");

                    continue;
                }

                copiedFiles.Add(file.AbsolutePath);
                file.MoveTo(targetPath);
            }
        });

        foreach (var file in files)
        {
            if (copiedFiles.Contains(file.AbsolutePath))
            {
                AnsiConsole.MarkupLine($"[green]{file.AbsolutePath}[/] was copied.");
            }
            else if (skippedFiles.Contains(file.AbsolutePath))
            {
                AnsiConsole.MarkupLine($"[yellow]{file.AbsolutePath}[/] was skipped.");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{file.AbsolutePath}[/] has unknown status.");
            }
        }

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}
