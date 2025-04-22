using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class BaseSettings : CommandSettings
{
    [CommandOption($"-w|--workdir")]
    public string WorkDir { get; set; } = "";

    [CommandOption($"-p|--photosdir")]
    public string PhotosDir { get; set; } = "";

    public override string ToString()
    {
        return $"[yellow]WorkDir[/]: {WorkDir}\r\n[yellow]PhotosDir[/]: {PhotosDir}";
    }
}
