using Bild.Core.Features.Files;
using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class ConfigureCommand : Command<Cli>
{
    public static string Name => "[yellow]Configure App Defaults[/]";

    public override int Execute(CommandContext context, Cli _)
    {
        LoadConfigurationInteractor loadConfiguration = new();
        var settings = loadConfiguration.Perform();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Expand()
            .BorderColor(Color.Grey)
            .AddColumn("[cyan]Variable[/]")
            .AddColumn("[grey]Current Value[/]")
            .AddRow([nameof(settings.PhotosDir), settings.PhotosDir]);

        AnsiConsole.Write(table);

        if (AnsiConsole.Prompt(new ConfirmationPrompt($"Keep settings?")))
            return 0;

        AnsiConsole.MarkupLine("\r\nWhere is your photos library?");
        settings.PhotosDir = $"{PickDirectory(new MediaDir(settings.PhotosDir))}";      
        
        SaveConfigurationInteractor saveConfiguration = new();
        saveConfiguration.Perform(settings);

        return 0;
    }

    private MediaDir PickDirectory(MediaDir directory)
    {
        if (!directory.Exists)
        {
            var prompt = new ConfirmationPrompt($"Current selection: [red]" +
                $"{directory}[/]. Keep this value?");

            if (AnsiConsole.Prompt(prompt))
                return directory;
        }

        PathSelectorInteractor pathSelector = new();
        return pathSelector.Perform();
    }
}
