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

        AnsiConsole.MarkupLine("Current setting:");
        AnsiConsole.MarkupLine(baseSettings.ToString());
        if (!AnsiConsole.Prompt(new ConfirmationPrompt($"Change settings?")))
            return 0;

        AnsiConsole.MarkupLine("Where is your photos library?");
        baseSettings.PhotosDir = PickDirectory(baseSettings.PhotosDir);

        AnsiConsole.MarkupLine("Where should we look for new photos?");
        baseSettings.WorkDir = PickDirectory(baseSettings.WorkDir);

        SaveBaseSettingsInteractor saveBaseSettings = new();
        saveBaseSettings.Perform(baseSettings);

        return 0;
    }

    private string PickDirectory(string defaultValue)
    {
        if (!string.IsNullOrEmpty(defaultValue))
        {
            var prompt = new ConfirmationPrompt($"> [red]{defaultValue}[/].\r\n  Pick something else?");
            if (!AnsiConsole.Prompt(prompt))
                return defaultValue;
        }

        DriveSelectorInteractor driveSelector = new();
        var selectedDrive = driveSelector.Perform();

        if (string.IsNullOrEmpty(selectedDrive))
            return string.Empty;

        DirectorySelectorInteractor directorySelector = new();
        var selectedDir = directorySelector.Perform(selectedDrive);

        return selectedDir;
    }
}
