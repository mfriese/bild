using MetadataExtractor.Formats.FileType;

namespace Bild.Core.Interactors.EXIF;

public class GetFileNameExtensionInteractor
{
    public string Perform(IReadOnlyList<MetadataExtractor.Directory> exif)
    {
        return exif.OfType<FileTypeDirectory>().FirstOrDefault()?.
            GetDescription(FileTypeDirectory.TagExpectedFileNameExtension);
    }
}
