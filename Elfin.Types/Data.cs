using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace Elfin.Types
{

    public class ElfinData
    {
        public required string Token;
        public required DiscordIntents Intents;
        public required LogLevel LogLevel;
        public required string Prefix;
    }
}