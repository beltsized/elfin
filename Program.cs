using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using dotenv.net;

namespace Elfin
{
    public class Program
    {
        static void Main(string[] args)
        {
            DotEnv.Load();
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var ElfinClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("DISCORDTOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.None
            });

            ElfinClient.Ready += async (Self, Event) =>
            {
                Console.WriteLine("Elfin Is Ready!!");
            };


            ElfinClient.MessageCreated += async (Self, Event) =>
            {
                _ = Task.Run(async () =>
                {
                    if (Event.Message.Content.ToLower() == "elf.ping")
                        await Event.Message.RespondAsync("Pong!");
                });
            };

            await ElfinClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}