using Elfin.Attributes;
using Elfin.Types;
using Elfin.Core;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Text;
using Owoify;

namespace Elfin.Commands
{
    [ElfinGroup("fun")]
    public class ElfinFunCommandGroup
    {
        [ElfinCommand("helloworld")]
        [ElfinAliases(new string[] { "hworld" })]
        [ElfinUsage("[]")]
        [ElfinDescription("Hello, world!")]
        public static async Task HelloWorld(ElfinClient elfin, ElfinCommandContext context)
        {
            await context.Message.RespondAsync("Hello, world!");
        }

        [ElfinCommand("neko")]
        [ElfinAliases(new string[] { "nekoimg", "nekoimage" })]
        [ElfinUsage("[]")]
        [ElfinDescription("Sends a random image of a neko.")]
        public static async Task Neko(ElfinClient elfin, ElfinCommandContext context)
        {
            HttpResponseMessage response = await elfin.HttpClient.GetAsync("https://nekos.life/api/v2/img/neko");
            string rawResponse = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(rawResponse);

            await context.Message.RespondAsync(data!.url);
        }

        [ElfinCommand("8ball")]
        [ElfinAliases(new string[] { "8b" })]
        [ElfinUsage("[question]")]
        [ElfinDescription("Lets the 8ball determine your decision.")]
        public static async Task EightBall(ElfinClient elfin, ElfinCommandContext context)
        {
            if (context.Args.Length == 0)
            {
                await context.Message.RespondAsync("You must ask a question as well.");
            }
            else
            {
                HttpResponseMessage response = await elfin.HttpClient.GetAsync("https://nekos.life/api/v2/img/8ball");
                string rawData = await response.Content.ReadAsStringAsync();
                dynamic? data = JsonConvert.DeserializeObject(rawData);

                await context.Message.RespondAsync(data!.url);
            }
        }

        [ElfinCommand("owoify")]
        [ElfinAliases(new string[] { "owo" })]
        [ElfinUsage("[text]")]
        [ElfinDescription("Let the OwO gods decide your next words.")]
        public static async Task Owoify(ElfinClient elfin, ElfinCommandContext context)
        {
            if (context.Args.Length == 0)
            {
                await context.Message.RespondAsync("You must provide some form of text.");
            }
            else
            {
                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#2F3136"),
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        Name = Owoifier.Owoify(string.Join(" ", context.Args), Owoifier.OwoifyLevel.Uvu),
                        IconUrl = context.Author.AvatarUrl
                    }
                };

                await context.Message.RespondAsync(embed.Build());
            }
        }

        [ElfinCommand("animecharacter")]
        [ElfinAliases(new string[] { "anichar", "animechar" })]
        [ElfinUsage("[character name]")]
        [ElfinDescription("Sends information on an anime character.")]
        public static async Task AnimeCharacter(ElfinClient elfin, ElfinCommandContext context)
        {
            if (context.Args.Length == 0)
            {
                await context.Message.RespondAsync("You must provide a character's name.");
            }
            else
            {
                string searchGraphQL = @"
                    query ($search: String) {
                        characters: Page (perPage: 1) {
                            results: characters (search: $search) { id }
                        }
                    }
                ";

                string characterGraphQL = @"
                    query ($id: Int!) {
                        Character(id: $id) {
                            id
                            name {
                                full
                            }
                            image {
                                large
                                medium
                            }
                            gender
                            age
                            bloodType
                            favourites
                            description(asHtml: false)
                            siteUrl
                            media(page: 1, perPage: 25) {
                                edges {
                                    id
                                }
                            }
                        }
                    }
                ";

                string characterName = string.Join(" ", context.Args);
                dynamic? sJSON = JsonConvert.SerializeObject(new
                {
                    variables = new { search = characterName },
                    query = searchGraphQL
                });

                StringContent sBody = new(sJSON, Encoding.UTF8, "application/json");
                HttpResponseMessage sResponse = await elfin.HttpClient.PostAsync("https://graphql.anilist.co/", sBody);
                string sRawData = await sResponse.Content.ReadAsStringAsync();

                if (sRawData == "")
                {
                    await context.Message.RespondAsync("No character found.");
                }
                else
                {
                    dynamic? sData = JsonConvert.DeserializeObject(sRawData);

                    if (sData!.data.characters.results.Count == 0)
                    {
                        await context.Message.RespondAsync("No character found.");
                    }
                    else
                    {
                        string characterId = sData!.data.characters.results[0].id;
                        dynamic? fJSON = JsonConvert.SerializeObject(new
                        {
                            variables = new { id = characterId },
                            query = characterGraphQL
                        });

                        StringContent fBody = new(fJSON, Encoding.UTF8, "application/json");
                        HttpResponseMessage fResponse = await elfin.HttpClient.PostAsync("https://graphql.anilist.co/", fBody);
                        string fRawData = await fResponse.Content.ReadAsStringAsync();
                        dynamic? fData = JsonConvert.DeserializeObject(fRawData);
                        dynamic? character = fData!.data.Character;
                        dynamic? imageData = character.image;
                        string? image = imageData.large == null
                            ? (imageData.medium == null ? null : imageData.medium)
                            : imageData.large;
                        string siteUrl = character.siteUrl;
                        string fullName = $"{character.name.full}";

                        DiscordEmbedBuilder.EmbedAuthor author = new()
                        {
                            Name = "AniList",
                            IconUrl = "https://i.imgur.com/iUIRC7v.png",
                            Url = "https://anilist.co"
                        };

                        DiscordEmbedBuilder.EmbedThumbnail thumbnail = new()
                        {
                            Url = image
                        };

                        Page[] pages = {
                            new("", new DiscordEmbedBuilder() {
                                Color = new DiscordColor("#2F3136"),
                                Author = author,
                                Url = siteUrl,
                                Title = fullName,
                                Thumbnail = thumbnail,
                                Description = $@"
                                    Id: `{character.id}`
                                    Films: `{character.media.edges.Count}`
                                    Gender: `{character.gender}`
                                    Age: `{(character.age == null ? "N/A" : character.age)}`
                                    Favourites: `{character.favourites}`
                                    Blood type: `{(character.bloodType == null ? "N/A" : character.bloodType)}`
                                "
                            }),
                            new("", new DiscordEmbedBuilder() {
                                Color = new DiscordColor("#2F3136"),
                                Author = author,
                                Thumbnail = thumbnail,
                                Url = siteUrl,
                                Title = fullName,
                                Description = $"{character.description.Value.ToString().Substring(0, 2043)}`...`"
                            })
                        };

                        InteractivityExtension interactivity = elfin.RawClient.GetInteractivity();

                        await context.Channel.SendPaginatedMessageAsync(
                            context.Author, pages.Cast<Page>(),
                            behaviour: PaginationBehaviour.WrapAround,
                            deletion: ButtonPaginationBehavior.Disable
                        );
                    }
                }
            }
        }

        [ElfinCommand("safebooru")]
        [ElfinAliases(new string[] { "hi" })]
        public static async Task SafeBooru(ElfinClient elfin, ElfinCommandContext context)
        {
            if (context.Args.Length == 0)
            {
                await context.Message.RespondAsync("You must provide a tag or multiple tags.");
            }
            else
            {
                string query = HttpUtility.UrlEncode(string.Join(" ", context.Args));
                HttpResponseMessage response = await elfin.HttpClient.GetAsync($"https://safebooru.org/index.php?page=dapi&s=post&q=index&json=1&tags={query}&limit=1000");
                string rawData = await response.Content.ReadAsStringAsync();

                if (rawData == "")
                {
                    await context.Message.RespondAsync("No images found.");
                }
                else
                {
                    dynamic? data = JsonConvert.DeserializeObject(rawData);
                    Random random = new();
                    dynamic? pick = data![random.Next(0, data.Count)];

                    await context.Message.RespondAsync($"https://safebooru.org/images/{pick.directory}/{pick.image}");
                }
            }
        }
    }
}