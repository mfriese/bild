using System.Globalization;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using SharpExifTool;
using Directory = MetadataExtractor.Directory;

namespace Bild.Core.Interactors.EXIF;

public class GetCreationDateMp4Interactor
{
    public DateTime? Perform(string absolutePath)
    {
        using var et = new ExifTool();

        var metadata = et.ExtractAllMetadata(absolutePath);

        var exists = metadata.Any(mm => mm.Key == "QuickTime:CreateDate");
        
        // Zugriff auf CreateDate
        if (exists)
        {
            var mediaCreateRaw = metadata.
                FirstOrDefault(mm => mm.Key == "QuickTime:CreateDate");

            if (DateTime.TryParseExact(mediaCreateRaw.Value,
                    "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                if (dt < DateTime.Now)
                    return dt;
            }
        }

        return null;
    }
}
