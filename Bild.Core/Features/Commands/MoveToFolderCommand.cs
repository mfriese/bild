using System.Security.Cryptography;
using Bild.Core.Features.Files;
using Bild.Core.Features.Importer;
using Bild.Core.Interactors.Files;
using Bild.Core.Interactors.Hashing;
using Bild.Core.Interactors.Settings;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class MoveToFolderCommand : Command<MoveToFolderSettings>
{
    public static string Name => "Move from import folder to correct subfolder";
    
    public override int Execute(CommandContext context, MoveToFolderSettings settings)
    {
        GetImportPathInteractor getImportPath = new();
        var sourcePath = getImportPath.Perform(settings);

        GetLibraryPathInteractor getLibraryPath = new();
        var targetPath = getLibraryPath.Perform(settings);
        
        var files = Finder.FindFiles(sourcePath?.AbsolutePath);

        var md5 = MD5.Create();
        GetB36HashInteractor getB36Hash = new();
        
        foreach (var file in files)
        {
            if (!file.IsAccepted)
            {
                AnsiConsole.MarkupLine($"[red]{file.AbsolutePath}[/]File is not accepted.");
                
                continue;
            }

            var creationDate = file.ExifCreationDate;

            var year = creationDate?.ToString("yyyy") ?? string.Empty;
            var month = creationDate?.ToString("MM") ?? string.Empty;

            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
            {
                AnsiConsole.MarkupLine($"[red]{file.AbsolutePath}[/]has no exif information.");
                
                continue;
            }
            
            var targetRootDir = targetPath?.AbsolutePath ?? string.Empty;
            var targetYearDir = Path.Combine(targetRootDir, year);
            var targetDir = Path.Combine(targetYearDir, month);;
            
            if (!Directory.Exists(targetYearDir))
                Directory.CreateDirectory(targetYearDir);
            
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);
            
            // var hashSuffix = getB36Hash.Perform(md5, file.AbsolutePath);

            var filename = "img_" + creationDate?.ToString("yyyyMMdd_hhmmss");
            var extension = file.exifFileNameExtension ?? file.Extension;

            AnsiConsole.MarkupLine($"Moving [red]{file.AbsolutePath}[/] to [red]{targetDir}[/].");
            
            file.MoveTo(new MediaDir(targetDir));

            // GetShortHash(file);
            // GetFilename(file, shortHash);
            // GetTargetPath();
            // MoveToPath(file, targetPath);
        }

        WaitKeyPressInteractor waitKeyPress = new();
        return waitKeyPress.Perform(0);
    }
}