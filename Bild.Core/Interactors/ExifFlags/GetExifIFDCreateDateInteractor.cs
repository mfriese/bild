using Bild.Core.Features.Files;
using System.Globalization;

namespace Bild.Core.Interactors.ExifFlags;

public class GetExifIFDCreateDateInteractor
{
    public DateTime? Perform(MediaFile file)
    {
        var mediaCreateRaw = file.Exif.
            FirstOrDefault(mm => mm.Key == "ExifIFD:CreateDate");

        if (DateTime.TryParseExact(mediaCreateRaw.Value,
            "yyyy:MM:dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var dt))
        {
            return dt;
        }

        return null;
    }
}
