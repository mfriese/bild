using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileSystem;

namespace Bild.Core.Interactors.EXIF;

public class GetCreationDateJpgInteractor
{
    public DateTime? Perform(IReadOnlyList<MetadataExtractor.Directory> exif)
    {
        var tagDateTime = exif.
            OfType<ExifIfd0Directory>().FirstOrDefault()?.
            GetDateTime(ExifDirectoryBase.TagDateTime);

        if (tagDateTime is not null)
        {
            return tagDateTime;
        }

        return exif.
            OfType<FileMetadataDirectory>().FirstOrDefault()?.
            GetDateTime(FileMetadataDirectory.TagFileModifiedDate);
    }
}
