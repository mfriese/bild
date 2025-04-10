using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace Bild.Core.Interactors.EXIF;

public class GetCreationDateJpgInteractor
{
    public DateTime? Perform(IReadOnlyList<MetadataExtractor.Directory> exif)
    {
        return exif.
            OfType<ExifIfd0Directory>().FirstOrDefault()?.
            GetDateTime(ExifDirectoryBase.TagDateTime);
    }
}
