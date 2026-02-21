using Bild.Core.Features.Files;
using Bild.Core.Interactors.UI;
using Spectre.Console;

namespace Bild.Core.Interactors.Directories;

public class DirectorySelectorInteractor
{
    private const string Cancel = "[red]Cancel[/]";
    private const string Accept = "[green]Accept[/]";

    public MediaDir Perform(MediaDir directory)
    {
        if (!directory.Exists)
        {
            AnsiConsole.MarkupLine($"[red]Selected path '{directory}' does not exist![/]");

            return directory;
        }

        List<string> directories = [];

        try
        {
            directories = directory.Dirs.Select(md => md.ToString()).Order().ToList();
        }
        catch (Exception exp)
        {
            AnsiConsole.MarkupLine($"[red]Error while reading directory![/]");
            AnsiConsole.WriteLine($"\r\n-> {exp.Message}\r\n");

            WaitKeyPressInteractor waitKeyPress = new();
            return waitKeyPress.Perform(directory);
        }

        directories.Add(Accept);
        directories.Add(Cancel);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Pick a subfolder of {directory} and {Accept} or {Cancel}.")
                .PageSize(16)
                .EnableSearch()
                .MoreChoicesText($"[grey](Navigate with arrow keys. Pick {Accept} or {Cancel} from the bottom)[/]")
                .AddChoices(directories)
        );

        if (selected == Cancel)
        {
            return new MediaDir(string.Empty);
        }

        if (selected == Accept)
        {
            return directory;
        }

        return directory.Dirs.First(dd => dd.ToString() == selected);
    }
}
