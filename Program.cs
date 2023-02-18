using Elfin.Types;
using Elfin.Core;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using dotenv.net;

namespace Elfin
{
    class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load();
            ElfinProcess().GetAwaiter().GetResult();
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

            elfin.Login().GetAwaiter().GetResult();
        }
    }
}