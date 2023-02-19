using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinCommandContext
    {
        public required DiscordMessage Message { get; init; }
        public required string[] Args { get; init; }
    }
}