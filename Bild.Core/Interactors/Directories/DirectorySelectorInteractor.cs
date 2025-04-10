using Spectre.Console;

namespace Bild.Core.Interactors.Directories;

public class DirectorySelectorInteractor
{
    private const string Cancel = "[red]Cancel[/]";
    private const string Accept = "[green]Accept[/]";

    public string Perform(string rootPath)
    {
        if (!Directory.Exists(rootPath))
        {
            AnsiConsole.MarkupLine($"[red]Selected path '{rootPath}' does not exist![/]");

            return string.Empty;
        }

        var directories = Directory.
            GetDirectories(rootPath).
            Select(Path.GetFileName).
            ToList();

        directories.Add(Accept);
        directories.Add(Cancel);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Pick a subfolder of {rootPath} and {Accept} or {Cancel}.")
                .PageSize(16)
                .EnableSearch()
                .MoreChoicesText($"[grey](Navigate with arrow keys. Pick {Accept} or {Cancel} from the bottom)[/]")
                .AddChoices(directories)
        );

        if (selected == Cancel)
        {
            return string.Empty;
        }

        if (selected == Accept)
        {
            return rootPath;
        }

        return Perform(Path.Combine(rootPath, selected));
    }

}