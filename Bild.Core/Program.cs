using Bild.Core.Features.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core;

public class Program
{
    private const string Cancel = "[red]Cancel[/]";

    private CommandApp MakeApp()
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.AddCommand<DuplicatesCommand>(DuplicatesCommand.Name);
        });

        return app;
    }

    public void Execute()
    {
        var app = MakeApp();

        var options = new string[]
        {
            DuplicatesCommand.Name,
            Cancel
        };

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
            new FigletText("Bild App")
                .Centered()
                .Color(Color.White));

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Tools to organize photos and videos. Pick a command ...")
                    // .PageSize(16)
                    .MoreChoicesText($"[grey](Navigate with arrow keys.)[/]")
                    .AddChoices(options)
            );

            if (selected == Cancel)
            {
                if (AnsiConsole.Prompt(
                    new ConfirmationPrompt("Sure you want to exit?")))
                {
                    AnsiConsole.MarkupLine("[red]Goodbye![/]");

                    return;
                }
            }
            else
            {
                app.Run([selected]);
            }
        }
    }
}
