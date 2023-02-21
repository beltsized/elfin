using Elfin.Types;
using Elfin.Core;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using dotenv.net;

namespace Elfin
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            DotEnv.Load();
            await ElfinProcess();
        }

        public static async Task ElfinProcess()
        {
            var elfin = new ElfinClient(new ElfinData()
            {
                Token = Environment.GetEnvironmentVariable("DISCORDTOKEN")!,
                Intents = DiscordIntents.All,
                LogLevel = LogLevel.None,
                Prefix = "!"
            });

            elfin.LoadCommands();
            elfin.LoadEvents();

            await elfin.Login();
        }
    }
}