using DSharpPlus.EventArgs;

namespace Elfin.Types
{
    public class ElfinCommandContext
    {
        public required MessageCreateEventArgs Packet { get; init; }
        public required string[] Args { get; init; }
    }
}