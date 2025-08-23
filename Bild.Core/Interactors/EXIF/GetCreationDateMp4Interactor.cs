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
        // ExifTool-Instanz (nutzt default exiftool.exe aus dem PATH)
        using var et = new ExifTool();

        // Metadaten asynchron auslesen
        var metadata = et.ExtractAllMetadata(absolutePath);

        var exists = metadata.Any(mm => mm.Key == "QuickTime:CreateDate");
        
        // Zugriff auf CreateDate
        if (exists)
        {
            var mediaCreateRaw = metadata.
                FirstOrDefault(mm => mm.Key == "QuickTime:CreateDate");
            
            Console.WriteLine($"Rohwert: {mediaCreateRaw}");

            if (DateTime.TryParseExact(mediaCreateRaw.Value,
                    "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                Console.WriteLine($"UTC:   {dt:u}");
                Console.WriteLine($"Local: {dt.ToLocalTime()}");

                return dt;
            }
            else
            {
                Console.WriteLine("Parsing fehlgeschlagen.");
            }
        }
        else
        {
            Console.WriteLine("MediaCreateDate nicht vorhanden.");
        }

        return null;
    }
}
