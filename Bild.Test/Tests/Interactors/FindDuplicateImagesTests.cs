using Bild.Core.Features.Files;
using Bild.Core.Interactors.Files;
using Bild.Test.Interactors;

namespace Bild.Test.Tests.Interactors;

public class FindDuplicateImagesTests
{
    [Fact]
    public void Perform_PrefersTheFileWithTheShorterNameForIdenticalImages()
    {
        GetCurrentPathInteractor getCurrentPath = new();
        var samplePath = Path.Combine(getCurrentPath.Perform(), "Samples", "pic_01.jpg");
        var temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            var preferredPath = Path.Combine(temporaryDirectory, "image.jpg");
            var duplicatePath = Path.Combine(temporaryDirectory, "image_01.jpeg");
            File.Copy(samplePath, preferredPath);
            File.Copy(samplePath, duplicatePath);

            FindDuplicateImagesInteractor findDuplicates = new();
            var duplicates = findDuplicates.Perform([new MediaFile(duplicatePath), new MediaFile(preferredPath)]);

            var duplicate = Assert.Single(duplicates);
            Assert.True(duplicate.IsExactMatch);
            Assert.Equal(preferredPath, duplicate.FileToKeep.ToString());
            Assert.Equal(duplicatePath, duplicate.FileToDelete.ToString());
        }
        finally
        {
            Directory.Delete(temporaryDirectory, true);
        }
    }
}
