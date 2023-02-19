using Elfin.Attributes;
using Elfin.Types;
using Elfin.Core;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Elfin.Commands
{
    [ElfinGroup("fun")]
    public class ElfinFunCommandGroup
    {
        [ElfinCommand("helloworld")]
        [ElfinAliases(new string[] { "hworld" })]
        [ElfinDescription("Hello, world!")]
        public static async Task HelloWorld(ElfinClient elfin, ElfinCommandContext context)
        {
            await context.Packet.Message.RespondAsync("Hello, world!");
        }

        [ElfinCommand("neko")]
        [ElfinAliases(new string[] { "nekoimg", "nekoimage" })]
        [ElfinDescription("Sends a random image of a neko.")]
        public static async Task Neko(ElfinClient elfin, ElfinCommandContext context)
        {
            HttpResponseMessage response = await elfin.HttpClient.GetAsync("https://nekos.life/api/v2/img/neko");
            string rawResponse = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(rawResponse);
            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#2F3136"),
                ImageUrl = data!.url
            };

            await context.Packet.Message.RespondAsync(embed.Build());
        }
    }
}