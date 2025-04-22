using Bild.Core.Features.Commands;
using System.Text.Json;

namespace Bild.Core.Interactors.Settings;

public class LoadBaseSettingsInteractor
{
    public ConfigureSettings Perform()
    {
        GetSettingsPathInteractor getSettingsPath = new();
        var settingsPath = getSettingsPath.Perform();

        if (!File.Exists(settingsPath))
            return new ConfigureSettings();

        var jsonText = File.ReadAllText(settingsPath);

        if (string.IsNullOrEmpty(jsonText))
            return new ConfigureSettings();

        var settings = JsonSerializer.Deserialize<ConfigureSettings>(jsonText);

        return settings;
    }
}
