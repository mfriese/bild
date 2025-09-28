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

        DateTime? quickTimeDate = null;
        DateTime? createdDate = null;
        
        // some cameras produce terribly wrong dates
        if (metadata.Any(mm => mm.Key == "QuickTime:CreateDate"))
        {
            var mediaCreateRaw = metadata.
                FirstOrDefault(mm => mm.Key == "QuickTime:CreateDate");

            if (DateTime.TryParseExact(mediaCreateRaw.Value,
                    "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                quickTimeDate = dt;
            }
        }

        // TODO: Fix this! This is just a fallback. But some files are seriously broken in that regard!
        if (metadata.Any(mm => mm.Key == "System:FileModifyDate"))
        {
            var mediaCreateRaw = metadata.
                FirstOrDefault(mm => mm.Key == "System:FileModifyDate");

            if (DateTime.TryParseExact(mediaCreateRaw.Value,
                    "yyyy:MM:dd HH:mm:ssK",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                createdDate = dt;
            }
        }

        // if within one year time range I supposed the dates a okay
        if (quickTimeDate != null && createdDate != null)
        {
            if (quickTimeDate.Value.AddYears(1) > createdDate &&
                createdDate.Value.AddYears(1) > quickTimeDate)
            {
                return quickTimeDate;
            }
        }

        // if quicktime create date is missing just use modified date - seems more robust
        if (createdDate != null)
        {
            return createdDate;
        }
        
        return null;
    }
}
