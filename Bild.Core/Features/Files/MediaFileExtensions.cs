using Spectre.Console;

namespace Bild.Core.Features.Files;

public static class MediaFileExtensions
{
    public static MediaFile Move(this MediaFile file, string targetDir, string targetTemplate)
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

        var targetName = file.ExifCreationDate?.ToString(targetTemplate) ?? string.Empty;

        if (string.IsNullOrEmpty(targetName))
        {
            AnsiConsole.MarkupLine($"[red]No filename can be created for {file.Filename}![/]");
            return null;
        }

        var targetExtension = file.ExifFileNameExtension ?? file.Extension;

        var targetFileName = $"{targetName}.{targetExtension}";
        var targetFilePath = Path.Combine(targetDir, targetFileName);

        for (int ii = 1; File.Exists(targetFilePath); ++ii)
        {
            targetFileName = $"{targetName}_{ii}.{targetExtension}";
            targetFilePath = Path.Combine(targetDir, targetFileName);
        }

        AnsiConsole.MarkupLine($"Moving file [green]{file.Filename}[/] to [green]{targetFilePath}[/]");

        File.Move(file.AbsolutePath, targetFilePath);

        return new MediaFile(targetFilePath);
    }
}
