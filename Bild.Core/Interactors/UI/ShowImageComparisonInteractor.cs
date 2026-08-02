using Bild.Core.Features.Files;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Bild.Core.Interactors.UI;

public class ShowImageComparisonInteractor
{
    public void Perform(MediaFile fileToKeep, MediaFile fileToDelete)
    {
        var imageWidth = Math.Max(1, 32);
        var imageHeight = Math.Max(1, 32);

        AnsiConsole.Write(new Columns(
        [
            CreatePanel(fileToKeep, "Keep", imageWidth, imageHeight),
            CreatePanel(fileToDelete, "Delete", imageWidth, imageHeight)
        ]));
    }

    private static Panel CreatePanel(MediaFile file, string action, int width, int height)
    {
        var content = CreatePreview(file, width, height);
        return new Panel(content)
        {
            Header = new PanelHeader(action),
            Border = BoxBorder.Rounded
        };
    }

    private static IRenderable CreatePreview(MediaFile file, int width, int height)
    {
        try
        {
            using var image = Image.Load<Rgba32>(file.ToString());
            var scale = Math.Min((double)width / image.Width, (double)height / image.Height);
            var previewWidth = Math.Max(1, (int)(image.Width * scale));
            var previewHeight = Math.Max(1, (int)(image.Height * scale));
            var canvas = new Canvas(previewWidth, previewHeight);

            for (var y = 0; y < previewHeight; y++)
            {
                var sourceY = Math.Min(image.Height - 1, y * image.Height / previewHeight);
                for (var x = 0; x < previewWidth; x++)
                {
                    var sourceX = Math.Min(image.Width - 1, x * image.Width / previewWidth);
                    var pixel = image[sourceX, sourceY];
                    canvas.SetPixel(x, y, new Spectre.Console.Color(pixel.R, pixel.G, pixel.B));
                }
            }

            return canvas;
        }
        catch (Exception)
        {
            return new Markup("[red]Preview could not be rendered.[/]");
        }
    }
}
