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
                Token = Environment.GetEnvironmentVariable("DISCORDTOKEN"),
                Intents = DiscordIntents.All,
                LogLevel = LogLevel.None,
                Prefix = "elf."
            });

            elfin.LoadCommands();

            elfin.RawClient.Ready += async (self, packet) =>
            {
                Console.WriteLine("Elfin Is Ready!!");
            };

            elfin.RawClient.MessageCreated += async (self, packet) =>
            {
                elfin.HandlePossibleCommand(packet);
            };

            await elfin.Login();
        }
    }
}