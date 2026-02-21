using Bild.Core.Features.Commands;
using System.Text.Json;
using Bild.Core.Features.Settings;

namespace Bild.Core.Interactors.Settings;

public class SaveConfigurationInteractor
{
    public void Perform(Configuration settings)
    {
        GetConfigurationPathInteractor getConfigurationPath = new();
        var settingsPath = getConfigurationPath.Perform();

        var jsonText = JsonSerializer.Serialize(settings);

        File.WriteAllText(settingsPath, jsonText);
    }
}
