using Bild.Core.Features.Commands;
using System.Text.Json;
using Bild.Core.Features.Settings;

namespace Bild.Core.Interactors.Settings;

public class LoadConfigurationInteractor
{
    public Configuration Perform()
    {
        GetConfigurationPathInteractor getConfigurationPath = new();
        var settingsPath = getConfigurationPath.Perform();

        if (!File.Exists(settingsPath))
            return new Configuration();

        var jsonText = File.ReadAllText(settingsPath);

        if (string.IsNullOrEmpty(jsonText))
            return new Configuration();

        var settings = JsonSerializer.Deserialize<Configuration>(jsonText);

        return settings;
    }
}
