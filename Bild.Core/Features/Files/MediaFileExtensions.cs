using Bild.Core.Interactors.Files;
using Spectre.Console;

namespace Bild.Core.Features.Files;

public static class MediaFileExtensions
{
    public static MediaFile CopyTo(this MediaFile file, MediaDir targetDir)
    {
        if (!File.Exists(file.AbsolutePath))
        {
            AnsiConsole.MarkupLine($"[red]Source file {file.Filename} does not exist![/]");
            return null;
        }

        var targetPath = targetDir.AbsolutePath;
        
        if (!Directory.Exists(targetPath))
        {
            AnsiConsole.MarkupLine($"[red]Target directory {targetDir} does not exist![/]");
            return null;
        }

        ResolveNameClashInteractor resolveNameClash = new();
        var targetExtension = file.ExifFileNameExtension ?? file.Extension;
        var targetFilename = resolveNameClash.Perform(targetPath, file.Filename, targetExtension);
        var targetFilePath = Path.Combine(targetPath, $"{targetFilename}.{targetExtension}"); 

        File.Copy(file.AbsolutePath, targetFilePath, false);

        return new MediaFile(targetFilePath);
    }
    
    public static MediaFile MoveTo(this MediaFile file, MediaDir targetDir)
    {
        if (!File.Exists(file.AbsolutePath))
        {
            AnsiConsole.MarkupLine($"[red]Source file {file.Filename} does not exist![/]");
            return null;
        }

        var targetPath = targetDir.AbsolutePath;
        
        if (!Directory.Exists(targetPath))
        {
            AnsiConsole.MarkupLine($"[red]Target directory {targetDir} does not exist![/]");
            return null;
        }

        ResolveNameClashInteractor resolveNameClash = new();
        var targetExtension = file.ExifFileNameExtension ?? file.Extension;
        var targetFilename = resolveNameClash.Perform(targetPath, file.Filename, targetExtension);
        var targetFilePath = Path.Combine(targetPath, $"{targetFilename}.{targetExtension}"); 

        // for testing
        File.Copy(file.AbsolutePath, targetFilePath, false);
        // File.Move(file.AbsolutePath, targetFilePath, false);

        return new MediaFile(targetFilePath);
    }

    public static MediaFile RenameToDateTemplate(this MediaFile file, string targetTemplate)
    {
        if (!File.Exists(file.AbsolutePath))
            return null;

        var targetName = file.ExifCreationDate?.ToString(targetTemplate) ?? string.Empty;

        if (string.IsNullOrEmpty(targetName))
            return null;

        targetName = "IMG_" + targetName;

        var targetExtension = file.ExifFileNameExtension ?? file.Extension;
        var targetDir = file.Dir.AbsolutePath;

        var targetFileName = $"{targetName}.{targetExtension}";
        var targetFilePath = Path.Combine(targetDir, targetFileName);

        for (int ii = 1; File.Exists(targetFilePath); ++ii)
        {
            if (Equals(file.AbsolutePath, targetFilePath))
                return null;

            targetFileName = $"{targetName}_{ii}.{targetExtension}";
            targetFilePath = Path.Combine(targetDir, targetFileName);
        }

        AnsiConsole.MarkupLine($"[green]Renamed[/] file [yellow]{file.Filename}[/] to " +
            $"[yellow]{targetFileName}[/]");

        File.Move(file.AbsolutePath, targetFilePath, false);

        return new MediaFile(targetFilePath);
    }
}
