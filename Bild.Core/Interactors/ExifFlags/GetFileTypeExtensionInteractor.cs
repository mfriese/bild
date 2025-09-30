using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.ExifFlags;

public class GetFileTypeExtensionInteractor
{
    public string Perform(MediaFile file)
    {
        var exif = file.Exif.
            FirstOrDefault(mm => mm.Key == "File:FileTypeExtension");

        return exif.Value ?? string.Empty;
    }
}
