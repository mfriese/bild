using Bild.Core.Features.Files;
using Bild.Core.Interactors.EXIF;
using Bild.Core.Interactors.ExifFlags;
using Bild.Core.Interactors.Files;
using Bild.Test.Interactors;
using MetadataExtractor.Util;

namespace Bild.Test.Tests.Interactors;

public class ExifTests
{
    [Theory]
    [InlineData(0, "img_20240305_134512.jpg")]
    [InlineData(1, "img_20240305_134512_01.jpg")]
    [InlineData(12, "img_20240305_134512_12.jpg")]
    public void FileName_Uses24HourTimestampAndDeterministicCollisionIndex(int collisionIndex, string expected)
    {
        GetExifFilenameInteractor getExifFilename = new();

        var filename = getExifFilename.Perform(
            new DateTime(2024, 3, 5, 13, 45, 12),
            ".jpg",
            collisionIndex);

        Assert.Equal(expected, filename);
    }

    [Fact]
    public void FileHash_IsEqualForEqualContentAndDifferentOtherwise()
    {
        GetFileHashInteractor getFileHash = new();
        using var first = new MemoryStream([1, 2, 3]);
        using var same = new MemoryStream([1, 2, 3]);
        using var different = new MemoryStream([3, 2, 1]);

        var firstHash = getFileHash.Perform(first);
        var sameHash = getFileHash.Perform(same);
        var differentHash = getFileHash.Perform(different);

        Assert.Equal(firstHash, sameHash);
        Assert.NotEqual(firstHash, differentHash);
    }

    [Fact]
    public void Test_Pic_01_jpg_Date_Scanner()
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", "pic_01.jpg"));

        GetExifIFDCreateDateInteractor getExifIfdCreateDate = new();
        var creationDate = getExifIfdCreateDate.Perform(file);

        Assert.NotNull(creationDate);
        Assert.Equal(2019, creationDate.Value.Year);
        Assert.Equal(3, creationDate.Value.Month);
        Assert.Equal(23, creationDate.Value.Day);
    }

    [Fact]
    public void Test_Vid_01_mov_Date_Scanner()
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", "vid_01.mov"));

        GetQuickTimeCreateDateInteractor getQuickTimeCreateDate = new();
        var creationDate = getQuickTimeCreateDate.Perform(file);

        Assert.NotNull(creationDate);
        Assert.Equal(2003, creationDate.Value.Year);
        Assert.Equal(3, creationDate.Value.Month);
        Assert.Equal(3, creationDate.Value.Day);
    }

    [Fact]
    public void Test_Vid_02_mp4_Date_Scanner()
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", "vid_02.mp4"));

        GetQuickTimeCreateDateInteractor getQuickTimeCreateDate = new();
        var creationDate = getQuickTimeCreateDate.Perform(file);

        Assert.NotNull(creationDate);
        Assert.Equal(2020, creationDate.Value.Year);
        Assert.Equal(2, creationDate.Value.Month);
        Assert.Equal(6, creationDate.Value.Day);
    }

    [Theory]
    [InlineData("pic_01.jpg", "jpg")]
    [InlineData("vid_01.mov", "mov")]
    [InlineData("vid_02.mp4", "mp4")]
    [InlineData("pic_02.arw", "arw")]
    [InlineData("pic_03.jpg", "jpg")]
    [InlineData("vid_03.avi", "avi")]
    [InlineData("vid_04.mp4", "mp4")]
    public void Test_FileExtensionFlag(string fileName, string extension)
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", fileName));

        GetFileTypeExtensionInteractor getFileTypeExtension = new();
        var fileExtension = getFileTypeExtension.Perform(file);

        Assert.Equal(extension, fileExtension);
    }

    [Theory]
    [InlineData("pic_01.jpg")]
    [InlineData("vid_01.mov")]
    [InlineData("vid_02.mp4")]
    [InlineData("pic_02.arw")]
    [InlineData("pic_03.jpg")]
    [InlineData("vid_03.avi")]
    [InlineData("vid_04.mp4")]
    public void Test_Date_Scanner_All_Files(string fileName)
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", fileName));

        GetCreationDateInteractor getCreationDateInteractor = new();
        var creationDate = getCreationDateInteractor.Perform(file);

        Assert.NotNull(creationDate);
    }
}
