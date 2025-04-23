using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class ConfigureCommand : Command<ConfigureSettings>
{
    public static string Name => "Configure App Defaults";

    public override int Execute(CommandContext context, ConfigureSettings settings)
    {
        LoadBaseSettingsInteractor loadBaseSettings = new();
        var baseSettings = loadBaseSettings.Perform();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Expand()
            .BorderColor(Color.Grey)
            .AddColumn("[cyan]Variable[/]")
            .AddColumn("[grey]Current Value[/]")
            .AddRow([nameof(baseSettings.PhotosDir), baseSettings.PhotosDir]);

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine(baseSettings.ToString());
        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"Change settings?")))
            return 0;

        AnsiConsole.MarkupLine("Where is your photos library?");
        baseSettings.PhotosDir = PickDirectory(baseSettings.PhotosDir);

        SaveBaseSettingsInteractor saveBaseSettings = new();
        saveBaseSettings.Perform(baseSettings);

        return 0;
    }

    private string PickDirectory(string defaultValue)
    {
        if (!string.IsNullOrEmpty(defaultValue))
        {
            var prompt = new ConfirmationPrompt($"Current selection: [red]" +
                $"{defaultValue}[/].\r\nKeep this value?");

            if (AnsiConsole.Prompt(prompt))
                return defaultValue;
        }

        PathSelectorInteractor pathSelector = new();
        return pathSelector.Perform();
    }
}
