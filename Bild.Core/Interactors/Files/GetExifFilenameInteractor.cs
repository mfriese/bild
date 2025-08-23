using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Files;

public class GetExifFilenameInteractor
{
    public string Perform(MediaFile file)
    {
        if (file.ExifCreationDate?.ToString("yyyyMMdd_hhmmss") is string date)
            return $"img_{date}.{file.ExifFileNameExtension ?? file.Extension}";
        return null;
    }
}
