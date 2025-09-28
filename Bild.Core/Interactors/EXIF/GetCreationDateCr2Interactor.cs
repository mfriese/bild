using System.Globalization;
using SharpExifTool;

namespace Bild.Core.Interactors.EXIF;

public class GetCreationDateCr2Interactor
{
    public DateTime? Perform(string absolutePath)
    {
        using var et = new ExifTool();

        var metadata = et.ExtractAllMetadata(absolutePath);
        
        // some cameras produce terribly wrong dates
        if (metadata.Any(mm => mm.Key == "ExifIFD:CreateDate"))
        {
            var mediaCreateRaw = metadata.
                FirstOrDefault(mm => mm.Key == "ExifIFD:CreateDate");

            if (DateTime.TryParseExact(mediaCreateRaw.Value,
                    "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                return dt;
            }
        }

        return null;
    }
}