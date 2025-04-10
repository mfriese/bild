using Bild.Core.Interactors.EXIF;
using MetadataExtractor;
using MetadataExtractor.Util;

namespace Bild.Core.Features.Files;

public class File(string path)
{
    public string AbsolutePath { get; } = Path.GetFullPath(path);

    public string Filename => Path.GetFileName(AbsolutePath);
    public string Extension => Path.GetExtension(AbsolutePath);

    public FileType? exifFileType;
    public FileType? ExifFileType => exifFileType ??= GetExifFileType();

    private DateTime? exifCreationDate;
    public DateTime? ExifCreationDate => exifCreationDate ??= GetExifCreationDate();

    public string? exifFileNameExtension;
    public string? ExifFileNameExtension => exifFileNameExtension ??= GetExifFileNameExtension();

    public DateTime? fileCreationDate;
    public DateTime? FileCreationDate => fileCreationDate ??= GetFileCreationDate();

    public Dir Dir => new(Path.GetDirectoryName(AbsolutePath));

    private FileType? GetExifFileType()
    {
        using var stream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);

        return FileTypeDetector.DetectFileType(stream);
    }

    private DateTime? GetExifCreationDate()
    {
        try
        {
            var exif = ImageMetadataReader.ReadMetadata(AbsolutePath);

            switch (ExifFileType)
            {
                case FileType.Jpeg:
                    GetCreationDateJpgInteractor getJpgCreationDate = new();
                    return getJpgCreationDate.Perform(exif);
                case FileType.Mp4:
                    GetCreationDateMp4Interactor getMp4CreationDate = new();
                    return getMp4CreationDate.Perform(exif);
                default:
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
            var exif = ImageMetadataReader.ReadMetadata(AbsolutePath);

            GetFileNameExtensionInteractor getFileNameExtension = new();
            return getFileNameExtension.Perform(exif);
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
            creation = System.IO.File.GetCreationTime(AbsolutePath);
        }
        catch (Exception)
        { }

        return creation;
    }
}
