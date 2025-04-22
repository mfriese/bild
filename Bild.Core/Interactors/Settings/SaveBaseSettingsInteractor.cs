using Bild.Core.Features.Commands;
using System.Text.Json;

namespace Bild.Core.Interactors.Settings;

public class SaveBaseSettingsInteractor
{
    public void Perform(ConfigureSettings settings)
    {
        GetSettingsPathInteractor getSettingsPath = new();
        var settingsPath = getSettingsPath.Perform();

        var jsonText = JsonSerializer.Serialize(settings);

        File.WriteAllText(settingsPath, jsonText);
    }
}
