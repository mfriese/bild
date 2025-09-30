using Bild.Core.Features.Files;
using Bild.Core.Interactors.ExifFlags;
using MetadataExtractor.Util;

namespace Bild.Core.Interactors.EXIF;

public class GetCreationDateInteractor
{
    public DateTime? Perform(MediaFile file)
    {
        try
        {
            GetExifFileTypeInteractor getExifFileType = new();
            var fileType = getExifFileType.Perform(file);

            switch (fileType)
            {
                case FileType.Jpeg:
                case FileType.Cr2:
                case FileType.Avi:
                    GetExifIFDCreateDateInteractor getJpgCreationDate = new();
                    return getJpgCreationDate.Perform(file);
                case FileType.Mp4:
                case FileType.QuickTime:
                    GetQuickTimeCreateDateSecInteractor getMp4CreationDate = new();
                    return getMp4CreationDate.Perform(file);
                default:
                    Console.WriteLine("Cannot identify");
                    return null;
            }
        }
        catch (Exception)
        {
        }

        return null;
    }
}
