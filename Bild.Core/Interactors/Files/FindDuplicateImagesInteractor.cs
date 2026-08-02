using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Files;

public class FindDuplicateImagesInteractor
{
    public const double SimilarityThreshold = 99.9;

    public IReadOnlyList<DuplicateImagePair> Perform(IEnumerable<MediaFile> files)
    {
        GetFileHashInteractor getFileHash = new();
        var filesWithHash = files
            .Where(file => file.Exists)
            .Select(file => new { File = file, Hash = TryGetHash(getFileHash, file) })
            .Where(entry => entry.Hash is not null)
            .ToList();

        var duplicates = new List<DuplicateImagePair>();
        var representatives = new List<(MediaFile File, string Hash)>();

        foreach (var hashGroup in filesWithHash.GroupBy(entry => entry.Hash))
        {
            var orderedFiles = hashGroup
                .OrderBy(entry => GetFileNameLength(entry.File))
                .ThenBy(entry => Path.GetFileName(entry.File.ToString()), StringComparer.Ordinal)
                .ThenBy(entry => entry.File.ToString(), StringComparer.Ordinal)
                .ToList();

            representatives.Add((orderedFiles[0].File, orderedFiles[0].Hash));

            duplicates.AddRange(orderedFiles.Skip(1).Select(file =>
                new DuplicateImagePair(
                    orderedFiles[0].File,
                    file.File,
                    orderedFiles[0].Hash,
                    file.Hash,
                    true,
                    100d)));
        }

        CompareImagesInteractor compareImages = new();

        for (var firstIndex = 0; firstIndex < representatives.Count; firstIndex++)
        {
            for (var secondIndex = firstIndex + 1; secondIndex < representatives.Count; secondIndex++)
            {
                var first = representatives[firstIndex];
                var second = representatives[secondIndex];

                if (first.File.ExifCreationDate != second.File.ExifCreationDate)
                    continue;
                
                var similarity = compareImages.Perform(first.File, second.File);

                if (similarity < SimilarityThreshold)
                    continue;

                var (fileToKeep, fileToDelete) = OrderByFilenameLength(first, second);
                duplicates.Add(new DuplicateImagePair(
                    fileToKeep.File,
                    fileToDelete.File,
                    fileToKeep.Hash,
                    fileToDelete.Hash,
                    false,
                    similarity));
            }
        }

        return duplicates;
    }

    private static int GetFileNameLength(MediaFile file)
        => Path.GetFileName(file.ToString()).Length;

    private static ((MediaFile File, string Hash) FileToKeep, (MediaFile File, string Hash) FileToDelete) OrderByFilenameLength(
        (MediaFile File, string Hash) first,
        (MediaFile File, string Hash) second)
        => GetFileNameLength(first.File) < GetFileNameLength(second.File) ||
           GetFileNameLength(first.File) == GetFileNameLength(second.File) &&
           string.Compare(Path.GetFileName(first.File.ToString()), Path.GetFileName(second.File.ToString()), StringComparison.Ordinal) <= 0
            ? (first, second)
            : (second, first);

    private static string TryGetHash(GetFileHashInteractor getFileHash, MediaFile file)
    {
        try
        {
            return getFileHash.Perform(file);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
