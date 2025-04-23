using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class BaseSettings : CommandSettings
{
    [CommandOption($"-p|--photosdir")]
    public string PhotosDir { get; set; } = "";

    public override string ToString()
    {
        return $"[yellow]PhotosDir[/]: {PhotosDir}";
    }
}
