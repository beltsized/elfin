using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinCommand
    {
        public required string Name;
        public required string[] Aliases;
        public Action<DiscordMessage, string[]>? Respond;
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