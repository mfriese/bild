using Bild.Core.Features.Commands;
using Bild.Core.Interactors.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core;

public class Program
{
    private const string Cancel = "[red]Quit[/]";

    private CommandApp MakeApp()
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.AddCommand<NewImportCommand>(NewImportCommand.Name);
            config.AddCommand<ConfigureCommand>(ConfigureCommand.Name);
        });

        return app;
    }

    public void Execute()
    {
        var app = MakeApp();

        var options = new string[]
        {
            NewImportCommand.Name,
            ConfigureCommand.Name,
            Cancel
        };

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
            new FigletText("Bild App")
                .Color(Color.White));

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Tools to organize photos and videos. Pick a command ...")
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
                LoadBaseSettingsInteractor loadBaseSettings = new();
                var cfg = loadBaseSettings.Perform();

                app.Run([selected, "-p", cfg.PhotosDir]);
            }
        }
    }
}
