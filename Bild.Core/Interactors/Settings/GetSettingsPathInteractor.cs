namespace Bild.Core.Interactors.Settings;

public class GetSettingsPathInteractor
{
    public string Perform()
    {
        var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

        var settingsDirPath = Path.Combine(appData, "Bild");

        var settingsFilePath = Path.Combine(appData, "Bild", "settings.json");

        if (!Directory.Exists(settingsDirPath))
            Directory.CreateDirectory(settingsDirPath);

        return settingsFilePath;
    }
}
