using Bild.Core.Interactors.Files;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using CSharpFunctionalExtensions;

namespace Bild.Core.Features.Files;

public class MediaDir(string path)
{
    public const MediaDir Empty = null;

    public string AbsolutePath { get; } = Path.GetFullPath(path);

    public IEnumerable<MediaDir> Dirs
        => FindDirectories(AbsolutePath);

    public IEnumerable<MediaFile> Files
        => FindFiles(AbsolutePath);

    public MediaDir GetOrCreateSubdirectory(string subDir)
    {
        var subDirPath = Path.Combine(AbsolutePath, subDir);

        if (!Directory.Exists(subDirPath))
        {
            Directory.CreateDirectory(subDirPath);
        }

        return new MediaDir(subDirPath);
    }

    public Result<string> Insert(MediaFile file, bool randomSuffix = false)
    {
        GetExifFilenameInteractor getExifFilename = new();
        var targetFilename = getExifFilename.Perform(file, randomSuffix);

        if (targetFilename is null)
        {
            return Result.Failure<string>($"[red]Cannot determine target filename![/]");
        }

        var targetFilePath = Path.Combine(AbsolutePath, targetFilename);

        if (File.Exists(targetFilePath))
        {
            if (file.IsImage)
            {
                var hashAlgorithm = new AverageHash();
                using var sourceStream = File.OpenRead(file.AbsolutePath);
                using var targetStream = File.OpenRead(targetFilePath);
                ulong sourceHash = hashAlgorithm.Hash(sourceStream);
                ulong targetHash = hashAlgorithm.Hash(targetStream);
                double similarity = CompareHash.Similarity(sourceHash, targetHash);

                if (99.9 > similarity)
                {
                    // seems like a different file. Add a random suffix to avoid collision.
                    return Insert(file, true);
                }
                else
                {
                    // Similarity is 100% so it is the same file
                    return Result.Success($"[green]Similar file already exists, skipping.[/]");
                }
            }
            else
            {
                // Video cannot be compared this way
                return Result.Failure<string>($"[yellow]Video target file already exists.[/]");
            }
        }

        File.Copy(file.AbsolutePath, targetFilePath);

        return Result.Success("[green]File successfully copied![/]");
    }

    private static IEnumerable<MediaDir> FindDirectories(string absolutePath)
    {
        IEnumerable<MediaDir> findings;

        if (!Directory.Exists(absolutePath))
        {
            findings = [];
        }
        else
        {
            try
            {
                findings = Directory.
                    EnumerateDirectories(absolutePath).
                    Where(d => d != "." && d != "..").
                    Select(d => new MediaDir(d));
            }
            catch (UnauthorizedAccessException)
            {
                findings = [];
            }
        }

        return findings;
    }

    private static IEnumerable<MediaFile> FindFiles(string absolutePath)
    {
        IEnumerable<MediaFile> findings;

        if (!Directory.Exists(absolutePath))
        {
            // No findings, folder does not exist
            findings = [];
        }
        else
        {
            try
            {
                findings = Directory.
                    EnumerateFiles(absolutePath).
                    Select(f => new MediaFile(f));
            }
            catch (UnauthorizedAccessException)
            {
                findings = [];
            }
        }

        return findings;
    }

    public override string ToString()
        => AbsolutePath;
}
