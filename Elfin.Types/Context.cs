using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinCommandContext
    {
        public required MessageCreateEventArgs Packet { get; init; }
        public required DiscordUser Author { get; init; }
        public required DiscordGuild Guild { get; init; }
        public required DiscordChannel Channel { get; init; }
        public required DiscordMessage Message { get; init; }
        public required string[] Args { get; init; }
    }
}