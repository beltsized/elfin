using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinCommand
    {
        public required string Name;
        public required Action<DiscordMessage> Respond;

        public ElfinCommand()
        { }

        public ElfinCommand(ElfinCommand CommandData)
        {
            this.Name = CommandData.Name;
            this.Respond = CommandData.Respond;
        }
    }
}