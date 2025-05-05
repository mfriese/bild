using Bild.Core.Features.Commands;
using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Settings;

public class GetLibraryPathInteractor
{
    public MediaDir Perform(BaseSettings baseSettings = null)
    {
        if (baseSettings is null)
        {
            LoadBaseSettingsInteractor loadBaseSettings = new();
            baseSettings = loadBaseSettings.Perform();
        }

        return new MediaDir(baseSettings.PhotosDir);
    }
}
