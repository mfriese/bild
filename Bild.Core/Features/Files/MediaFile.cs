using Bild.Core.Interactors.EXIF;
using Bild.Core.Interactors.ExifFlags;
using MetadataExtractor.Util;
using SharpExifTool;

namespace Bild.Core.Features.Files;

public class MediaFile(string path)
{
    private ICollection<KeyValuePair<string, string>> exif;
    public ICollection<KeyValuePair<string, string>> Exif
        => exif ??= new ExifTool().ExtractAllMetadata(AbsolutePath);

    public string AbsolutePath { get; } = Path.GetFullPath(path);

    public string Extension => Path.GetExtension(AbsolutePath);

    public FileType? exifFileType;
    public FileType? ExifFileType => exifFileType ??= GetExifFileType();

    private DateTime? exifCreationDate;
    public DateTime? ExifCreationDate => exifCreationDate ??= GetExifCreationDate();

    public string exifFileNameExtension;
    public string ExifFileNameExtension => exifFileNameExtension ??= GetExifFileNameExtension();

    public bool IsAccepted
        => AcceptedTypes.Contains(GetExifFileType() ?? FileType.Unknown);

    private FileType? GetExifFileType()
    {
        GetExifFileTypeInteractor getExifFileType = new();
        return getExifFileType.Perform(this);
    }

    public bool IsImage
        => ExifFileType == FileType.Jpeg;

    private DateTime? GetExifCreationDate()
    {
        GetCreationDateInteractor getCreationDate = new();
        return getCreationDate.Perform(this);
    }

    private string GetExifFileNameExtension()
    {
        GetFileTypeExtensionInteractor getFileNameExtension = new();
        return getFileNameExtension.Perform(this);
    }

    public static FileType[] AcceptedTypes =>
    [
        FileType.QuickTime,
        FileType.Jpeg,
        FileType.Mp4,
        FileType.Cr2,
        FileType.Avi
    ];
}
