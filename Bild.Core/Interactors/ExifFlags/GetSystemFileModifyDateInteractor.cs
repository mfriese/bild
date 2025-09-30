using Bild.Core.Features.Files;
using System.Globalization;

namespace Bild.Core.Interactors.ExifFlags;

public class GetSystemFileModifyDateInteractor
{
    public DateTime? Perform(MediaFile file)
    {
        var mediaCreateRaw = file.Exif.
            FirstOrDefault(mm => mm.Key == "System:FileModifyDate");

        if (DateTime.TryParseExact(mediaCreateRaw.Value,
            "yyyy:MM:dd HH:mm:ssK",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var dt))
        {
            return dt;
        }

        return null;
    }
}
