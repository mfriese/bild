using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

namespace Bild.Core.Interactors.EXIF;

public class GetCreationDateMp4Interactor
{
    public DateTime? Perform(IReadOnlyList<MetadataExtractor.Directory> exif)
    {
        return exif.
            OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault()?.
            GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
    }
}
