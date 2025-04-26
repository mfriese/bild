using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Hashing;
using Bild.Core.Interactors.Settings;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Security.Cryptography;

namespace Bild.Core.Features.Commands;

public class ImportCommand : Command<ImportSettings>
{
    public static string Name => "Import files into photos library";

    public override int Execute(CommandContext context, ImportSettings settings)
    {
        AnsiConsole.MarkupLine($"Current setting for import. I will MOVE ALL " +
            "files from source to target folder. They will be remove from the " +
            "source folder and added to the target folder. Please select a " +
            "[red]source folder[/] first!\r\n");

        PathSelectorInteractor pathSelector = new();
        var sourcePath = pathSelector.Perform();

        if (string.IsNullOrEmpty(sourcePath))
            return 0;

        GetImportPathInteractor getImportPath = new();
        var targetPath = getImportPath.Perform(settings);

        var pp = new ConfirmationPrompt($"Import media from [red]{sourcePath}[/] " +
            $"to target folder [red]{targetPath}[/]");

        if (!AnsiConsole.Prompt(pp))
            return 1;

        var files = Finder.FindFiles(sourcePath);

        GetProgressInteractor getProgress = new();
        var progress = getProgress.Perform();

        progress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Count()} files[/]", maxValue: files.Count());

            foreach (var file in files)
            {
                if (string.IsNullOrEmpty(file.Filename))
                    continue;

                if (!file.IsAccepted)
                    continue;

                file.Move(targetPath);

                task.Increment(1);
            }
        });

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}
