using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public interface INamedCommand : ICommand
{
    public string CommandName { get; }
}
