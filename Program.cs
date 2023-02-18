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
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            var elfin = new ElfinClient(new ElfinClientData()
            {
                Token = Environment.GetEnvironmentVariable("DISCORDTOKEN"),
                Intents = DiscordIntents.All,
                LogLevel = LogLevel.None,
                Prefix = "elf."
            });

            elfin.SetCommands(new ElfinCommand[] {
                new ElfinCommand()
                {
                    Name = "ping",
                    Respond = async (message, args) =>
                    {
                        await message.RespondAsync("Pong!");
                    }
                }
            });

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