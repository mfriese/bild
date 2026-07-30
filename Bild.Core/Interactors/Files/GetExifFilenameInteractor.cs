using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Files;

public class GetExifFilenameInteractor
{
    public string Perform(MediaFile file, int collisionIndex = 0)
    {
        var extension = file.ExifFileNameExtension;

        if (string.IsNullOrWhiteSpace(extension))
            extension = file.Extension;

        return Perform(file.ExifCreationDate, extension, collisionIndex);
    }

    public string Perform(DateTime? creationDate, string extension, int collisionIndex = 0)
    {
        if (creationDate is null || string.IsNullOrWhiteSpace(extension) || collisionIndex < 0)
            return null;

        var suffix = collisionIndex == 0 ? string.Empty : $"_{collisionIndex:D2}";
        var normalizedExtension = extension.TrimStart('.');
        var date = creationDate.Value.ToString("yyyyMMdd_HHmmss");

        return $"img_{date}{suffix}.{normalizedExtension}";
    }
}
