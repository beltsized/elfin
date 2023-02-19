using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinCommand
    {
        public required string Name { get; init; }
        public required string[] Aliases { get; init; }
        public required Action<DiscordMessage, string[]>? Respond { get; init; }
        public bool Enabled = true;

        public ElfinCommand()
        { }

        public ElfinCommand(ElfinCommand commandData)
        {
            this.Name = commandData.Name;
            this.Aliases = commandData.Aliases;
        }
    }
}