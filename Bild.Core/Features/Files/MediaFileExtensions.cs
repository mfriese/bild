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

        File.Move(file.AbsolutePath, targetFilePath, false);

        return new MediaFile(targetFilePath);
    }
}
