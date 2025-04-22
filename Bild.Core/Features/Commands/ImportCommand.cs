using Bild.Core.Features.Importer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class ImportCommand : Command<ImportSettings>
{
    public static string Name => "Import all files into 'incoming' folder";

    public override int Execute(CommandContext context, ImportSettings settings)
    {
        AnsiConsole.MarkupLine($"Current setting for import. I will MOVE all files from source " +
            "to target folder. They will be remove from the source folder and added to the target " +
            "folder.\r\n");
        AnsiConsole.MarkupLine($"[cyan]Source folder[/]: {settings.WorkDir}");
        AnsiConsole.MarkupLine($"[cyan]Target folder[/]: {settings.PhotosDir}");
        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"[red]Continue[/]?")))
            return 1;

        var files = Finder.FindFiles(settings.WorkDir);

        return 0;
    }
}
