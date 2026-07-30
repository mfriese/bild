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
        var targetFilename = getExifFilename.Perform(sourceFile);

        if (targetFilename is null)
        {
            return Result.Failure<string>($"[red]Cannot determine target filename![/]");
        }

        var targetFile = new MediaFile(Path.Combine(AbsolutePath, targetFilename));
        var collisionIndex = 0;
        GetFileHashInteractor getFileHash = new();
        string sourceHash = null;

        while (targetFile.Exists)
        {
            sourceHash ??= getFileHash.Perform(sourceFile);
            var targetHash = getFileHash.Perform(targetFile);

            if (sourceHash is not null && sourceHash == targetHash)
            {
                return Result.Success($"[green]Identical file already exists, skipping.[/]");
            }

            if (sourceFile.IsImage)
            {
                CompareImagesInteractor compareImages = new();
                double similarity = compareImages.Perform(sourceFile, targetFile);

                if (similarity >= 99.9)
                {
                    return Result.Success($"[green]Similar file (factor '{similarity}') already exists, skipping.[/]");
                }
            }

            collisionIndex++;
            var collisionFilename = getExifFilename.Perform(sourceFile, collisionIndex);

            if (collisionFilename is null)
            {
                return Result.Failure<string>($"[red]Cannot determine target filename![/]");
            }

            targetFile = new MediaFile(Path.Combine(AbsolutePath, collisionFilename));
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
