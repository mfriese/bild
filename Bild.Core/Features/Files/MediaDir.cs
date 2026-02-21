using Bild.Core.Interactors.Files;
using CSharpFunctionalExtensions;

namespace Bild.Core.Features.Files;

public class MediaDir(string path)
{
    public bool Exists
        => !string.IsNullOrEmpty(path) && Directory.Exists(AbsolutePath);
    
    private string AbsolutePath
        => Path.GetFullPath(path);

    public IEnumerable<MediaDir> Dirs
        => FindDirectories(this);

    public IEnumerable<MediaFile> Files
        => FindFiles(this);

    public MediaDir GetOrCreateSubdirectory(string subDir)
    {
        var subDirPath = Path.Combine(AbsolutePath, subDir);

        if (!Directory.Exists(subDirPath))
        {
            Directory.CreateDirectory(subDirPath);
        }

        return new MediaDir(subDirPath);
    }

    public Result<string> Insert(MediaFile sourceFile)
    {
        GetExifFilenameInteractor getExifFilename = new();
        var targetFilenameWithSuffix = getExifFilename.Perform(sourceFile, true);
        var targetFilenameNoSuffix = getExifFilename.Perform(sourceFile, false);

        if (targetFilenameNoSuffix is null)
        {
            return Result.Failure<string>($"[red]Cannot determine target filename![/]");
        }

        var targetFile = new MediaFile(Path.Combine(AbsolutePath, targetFilenameNoSuffix));

        if (targetFile.Exists)
        {
            if (sourceFile.IsImage)
            {
                CompareImagesInteractor compareImages = new();
                double similarity = compareImages.Perform(sourceFile, targetFile);

                if (99.9 > similarity)
                {
                    // seems like a different file. Add a random suffix to avoid collision.
                    targetFile = new MediaFile(Path.Combine(AbsolutePath, targetFilenameWithSuffix));
                }
                else
                {
                    // Similarity is 100% so it is the same file
                    return Result.Success($"[green]Similar file (factor '{similarity}') already exists, skipping.[/]");
                }
            }
            else
            {
                // Video cannot be compared this way, but it's already there. With videos we assume
                // automatically it's the same. No need to copy.
                return Result.Success($"[yellow]Video target file '{targetFile}' already exists.[/]");
            }
        }

        targetFile.Copy(sourceFile);

        return Result.Success($"[green]File successfully copied to {targetFile}[/]");
    }

    private static IEnumerable<MediaDir> FindDirectories(MediaDir directory)
    {
        IEnumerable<MediaDir> findings;

        if (!directory.Exists)
        {
            findings = [];
        }
        else
        {
            try
            {
                findings = Directory.
                    EnumerateDirectories(directory.AbsolutePath).
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

    private static IEnumerable<MediaFile> FindFiles(MediaDir directory)
    {
        IEnumerable<MediaFile> findings;

        if (!directory.Exists)
        {
            // No findings, folder does not exist
            findings = [];
        }
        else
        {
            try
            {
                findings = Directory.
                    EnumerateFiles(directory.AbsolutePath).
                    Select(f =>
                    {
                        Console.Write(".");
                        return new MediaFile(f);
                    });
            }
            catch (UnauthorizedAccessException)
            {
                findings = [];
            }
        }

        return findings;
    }

    public override string ToString() => AbsolutePath;
}
