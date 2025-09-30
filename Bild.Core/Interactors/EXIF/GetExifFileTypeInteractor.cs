using Bild.Core.Features.Files;
using MetadataExtractor.Util;

namespace Bild.Core.Interactors.EXIF;

public class GetExifFileTypeInteractor
{
    public FileType Perform(MediaFile file)
    {
        using var stream = new FileStream(file.AbsolutePath, FileMode.Open, FileAccess.Read);

        return FileTypeDetector.DetectFileType(stream);
    }
}
