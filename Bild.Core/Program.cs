using Bild.Core.Features.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core;

public class Program
{
    private const string Cancel = "[red]Cancel[/]";

    private INamedCommand[] Commands { get; } =
    [
        new DuplicatesCommand(),
    ];

    public void Execute()
    {
        var names = Commands.
            Select(cc => cc.CommandName).
            Append(Cancel);

        while (true)
        {
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Pick the command you want to execute.")
                    .PageSize(16)
                    .MoreChoicesText($"[grey](Navigate with arrow keys.)[/]")
                    .AddChoices(names)
            );

            if (selected == Cancel)
            {
                AnsiConsole.MarkupLine("[red]Goodbye![/]");

                return;
            }

            var command = Commands.First(cc => cc.CommandName == selected);

            var app = new CommandApp<DuplicatesCommand>();
            app.Run([]);
        }
    }
}
