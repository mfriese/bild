using Bild.Core.Features.Files;
using Bild.Core.Interactors.EXIF;
using Bild.Core.Interactors.ExifFlags;
using Bild.Test.Interactors;
using MetadataExtractor.Util;

namespace Bild.Test.Tests.Interactors;

public class ExifTests
{
    [Fact]
    public void Test_Pic_01_jpg_Date_Scanner()
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", "pic_01.jpg"));

        GetExifIFDCreateDateInteractor getExifIFDCreateDate = new();
        var creationDate = getExifIFDCreateDate.Perform(file);

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
    [InlineData("pic_01.jpg", "jpg")]
    [InlineData("vid_01.mov", "mov")]
    [InlineData("vid_02.mp4", "mp4")]
    [InlineData("pic_02.arw", "arw")]
    [InlineData("pic_03.jpg", "jpg")]
    [InlineData("vid_03.avi", "avi")]
    [InlineData("vid_04.mp4", "mp4")]
    public void Test_ExifFileTypeScanner(string fileName, string extension)
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var path = getCurrentPath.Perform();

        MediaFile file = new(Path.Combine(path, "Samples", fileName));

        GetExifFileTypeInteractor getFileType = new();
        var fileExtension = getFileType.Perform(file);

        Assert.True(fileExtension.GetAllExtensions()?.Contains(extension) ?? false);
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
