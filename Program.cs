using System.Threading.Tasks;
using DSharpPlus;
using dotenv.net;


namespace Epic
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
            var BotClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("DISCORDTOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            BotClient.Ready += async (Self, Event) =>
            {
                Console.WriteLine("Elfin Is Ready!!");
            };


            BotClient.MessageCreated += async (Self, Event) =>
            {
                if (Event.Message.Content.ToLower() == "elf.ping")
                    await Event.Message.RespondAsync("Pong!");
            };

            await BotClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
