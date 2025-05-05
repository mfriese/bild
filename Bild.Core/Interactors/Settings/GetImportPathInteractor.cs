using Bild.Core.Features.Commands;
using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Settings;

public class GetImportPathInteractor
{
    public MediaDir Perform(BaseSettings baseSettings = null)
    {
        if (baseSettings is null)
        {
            LoadBaseSettingsInteractor loadBaseSettings = new();
            baseSettings = loadBaseSettings.Perform();
        }

        var importPath = Path.Combine(baseSettings.PhotosDir, "import");

        if (!Directory.Exists(importPath))
            Directory.CreateDirectory(importPath);

        return new MediaDir(importPath);
    }
}
