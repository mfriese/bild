using Bild.Core.Interactors.EXIF;
using MetadataExtractor;
using MetadataExtractor.Util;

namespace Bild.Core.Features.Files;

public class MediaFile(string path)
{
    private IReadOnlyList<MetadataExtractor.Directory> exif;
    private IReadOnlyList<MetadataExtractor.Directory> Exif
        => exif ??= ImageMetadataReader.ReadMetadata(AbsolutePath);

    public string AbsolutePath { get; } = Path.GetFullPath(path);

    public string Filename => Path.GetFileNameWithoutExtension(AbsolutePath);
    public string Extension => Path.GetExtension(AbsolutePath);

    public FileType? exifFileType;
    public FileType? ExifFileType => exifFileType ??= GetExifFileType();

    private DateTime? exifCreationDate;
    public DateTime? ExifCreationDate => exifCreationDate ??= GetExifCreationDate();

    public string exifFileNameExtension;
    public string ExifFileNameExtension => exifFileNameExtension ??= GetExifFileNameExtension();

    public DateTime? fileCreationDate;
    public DateTime? FileCreationDate => fileCreationDate ??= GetFileCreationDate();

    public MediaDir Dir => new(Path.GetDirectoryName(AbsolutePath));

    public bool IsAccepted
        => AcceptedTypes.Contains(GetExifFileType() ?? FileType.Unknown);

    private FileType? GetExifFileType()
    {
        using var stream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);

        return FileTypeDetector.DetectFileType(stream);
    }

    public bool IsImage
        => ExifFileType == FileType.Jpeg;

    private DateTime? GetExifCreationDate()
    {
        try
        {
            switch (ExifFileType)
            {
                case FileType.Jpeg:
                    GetCreationDateJpgInteractor getJpgCreationDate = new();
                    return getJpgCreationDate.Perform(Exif);
                case FileType.Mp4:
                case FileType.QuickTime:
                    GetCreationDateMp4Interactor getMp4CreationDate = new();
                    return getMp4CreationDate.Perform(Exif);
                default:
                    Console.WriteLine("Cannot identify");
                    return null;
            }
        }
        catch (Exception)
        { }

        return null;
    }

    private string GetExifFileNameExtension()
    {
        try
        {
            GetFileNameExtensionInteractor getFileNameExtension = new();
            return getFileNameExtension.Perform(Exif);
        }
        catch (Exception)
        { }

        return null;
    }

    private DateTime? GetFileCreationDate()
    {
        DateTime? creation = null;

        try
        {
            creation = File.GetCreationTime(AbsolutePath);
        }
        catch (Exception)
        { }

        return creation;
    }

    public static FileType[] AcceptedTypes =>
    [
        FileType.Jpeg,
        FileType.Mp4,
        FileType.QuickTime
    ];

    public override string ToString()
        => AbsolutePath;
}
