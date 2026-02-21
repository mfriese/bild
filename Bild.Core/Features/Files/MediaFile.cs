using Bild.Core.Interactors.EXIF;
using Bild.Core.Interactors.ExifFlags;
using MetadataExtractor.Util;
using SharpExifTool;

namespace Bild.Core.Features.Files;

public class MediaFile(string path)
{
    public bool Exists
        => !string.IsNullOrEmpty(path) && File.Exists(AbsolutePath);
    
    private string AbsolutePath { get; } = Path.GetFullPath(path);
    
    private ICollection<KeyValuePair<string, string>> _exif;
    public ICollection<KeyValuePair<string, string>> Exif
        => _exif ??= ExtractAll(AbsolutePath);

    private ICollection<KeyValuePair<string, string>> ExtractAll(string absPath)
    {
        using var exiftool = new ExifTool();
        return exiftool.ExtractAllMetadata(absPath);
    }
    
    public string Extension => Path.GetExtension(AbsolutePath);

    private FileType? _exifFileType;
    public FileType? ExifFileType
        => _exifFileType ??= GetExifFileType();

    private DateTime? _exifCreationDate;
    public DateTime? ExifCreationDate
        => _exifCreationDate ??= GetExifCreationDate();

    private string _exifFileNameExtension;
    public string ExifFileNameExtension
        => _exifFileNameExtension ??= GetExifFileNameExtension();

    public bool IsAccepted
        => AcceptedTypes.Contains(GetExifFileType() ?? FileType.Unknown);

    public bool IsImage
        => ExifFileType is FileType.Jpeg or FileType.Arw or FileType.Cr2;

    public void Copy(MediaFile source)
    {
        try
        {
            File.Copy(source.AbsolutePath, AbsolutePath, false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine($"Source: {source.AbsolutePath}");
            Console.WriteLine($"Target: {AbsolutePath}");
        }
    }

    public void Delete()
    {
        File.Delete(AbsolutePath);
    }

    public Stream ReadAsStream()
    {
        if (!Exists)
            return new MemoryStream();
        
        return File.OpenRead(AbsolutePath);
    }
    
    private FileType? GetExifFileType()
    {
        var fileInfo = new FileInfo(AbsolutePath);
        
        // do not use files smaller than 64kb
        if (fileInfo.Length < 1024 * 64)
            return FileType.Unknown;
        
        using var stream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);

        return FileTypeDetector.DetectFileType(stream, AbsolutePath);
    }
    
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

    private static FileType[] AcceptedTypes =>
    [
        FileType.QuickTime,
        FileType.Jpeg,
        FileType.Mp4,
        FileType.Arw,
        FileType.Cr2,
        FileType.Avi
    ];

    public override string ToString() => AbsolutePath;
}
