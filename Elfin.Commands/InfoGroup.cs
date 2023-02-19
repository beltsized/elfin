using Elfin.Attributes;
using Elfin.Types;
using Elfin.Core;
using DSharpPlus.Entities;
using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace Elfin.Commands
{
    [ElfinGroup("info")]
    public class ElfinInfoCommandGroup
    {
        [ElfinCommand("helloworld")]
        [ElfinAliases(new string[] { "hworld" })]
        [ElfinDescription("Hello, world!")]
        public static async Task HelloWorld(ElfinClient elfin, ElfinCommandContext context)
        {
            await context.Message.RespondAsync("Hello, world!");
        }

        [ElfinCommand("neko")]
        [ElfinAliases(new string[] { "nekoimg", "nekoimage" })]
        [ElfinDescription("Sends a random image of a neko.")]
        public static async Task Neko(ElfinClient elfin, ElfinCommandContext context)
        {
            var response = await elfin.HttpClient.GetAsync("https://nekos.life/api/v2/img/neko");
            var rawResponse = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(rawResponse);
            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#2F3136"),
                ImageUrl = data!.url
            };

            await context.Message.RespondAsync(embed.Build());
        }
    }
}