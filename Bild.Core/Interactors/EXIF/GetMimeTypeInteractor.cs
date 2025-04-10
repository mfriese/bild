using MetadataExtractor.Formats.FileType;

namespace Bild.Core.Interactors.EXIF;

public class GetMimeTypeInteractor
{
    public string Perform(IReadOnlyList<MetadataExtractor.Directory> exif)
    {
        return exif.OfType<FileTypeDirectory>().FirstOrDefault()?.
            GetDescription(FileTypeDirectory.TagDetectedFileMimeType);
    }
}
