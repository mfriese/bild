using Spectre.Console;

namespace Bild.Core.Features.Files;

public static class MediaFileExtensions
{
    public static MediaFile Move(this MediaFile file, string targetDir)
    {
        if (!File.Exists(file.AbsolutePath))
        {
            AnsiConsole.MarkupLine($"[red]Source file {file.Filename} does not exist![/]");
            return null;
        }

        if (!Directory.Exists(targetDir))
        {
            AnsiConsole.MarkupLine($"[red]Target directory {targetDir} does not exist![/]");
            return null;
        }

        var targetExtension = file.ExifFileNameExtension ?? file.Extension;

        var targetFileName = $"{file.Filename}.{targetExtension}";
        var targetFilePath = Path.Combine(targetDir, targetFileName);

        for (int ii = 1; File.Exists(targetFilePath); ++ii)
        {
            targetFileName = $"{file.Filename}_{ii}.{targetExtension}";
            targetFilePath = Path.Combine(targetDir, targetFileName);
        }

        File.Move(file.AbsolutePath, targetFilePath);

        return new MediaFile(targetFilePath);
    }

    public static MediaFile RenameToDateTemplate(this MediaFile file, string targetTemplate)
    {
        if (!File.Exists(file.AbsolutePath))
            return null;

        var targetName = file.ExifCreationDate?.ToString(targetTemplate) ?? string.Empty;

        if (string.IsNullOrEmpty(targetName))
            return null;

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
