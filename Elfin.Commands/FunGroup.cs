using Elfin.Attributes;
using Elfin.Types;
using Elfin.Core;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using System.Text.Json;
using System.Web;
using System.Net.Http.Json;
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
            var response = await elfin.HttpClient.GetFromJsonAsync<NekosLifeResponse>("https://nekos.life/api/v2/img/neko");

            await context.Message.RespondAsync(response!.Url);
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
                var response = await elfin.HttpClient.GetFromJsonAsync<NekosLifeResponse>("https://nekos.life/api/v2/img/8ball");

                await context.Message.RespondAsync(response!.Url);
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
                await context.Message.RespondAsync(Owoifier.Owoify(string.Join(" ", context.Args), Owoifier.OwoifyLevel.Uvu));
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
                var searchGraphQL = @"
                    query ($search: String) {
                        characters: Page (perPage: 1) {
                            results: characters (search: $search) { id }
                        }
                    }
                ";

                var characterGraphQL = @"
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

                var characterName = string.Join(" ", context.Args);
                var serialized = JsonSerializer.Serialize(new
                {
                    variables = new { search = characterName },
                    query = searchGraphQL
                });

                var payload = new StringContent(serialized, Encoding.UTF8, "application/json");
                var feed = await elfin.HttpClient.PostAsync("https://graphql.anilist.co/", payload);
                var raw = await feed.Content.ReadAsStringAsync();

                if (raw == "")
                {
                    await context.Message.RespondAsync("No character found.");
                }
                else
                {
                    var response = JsonSerializer.Deserialize<AniListResponse>(raw);

                    if (response!.Data!.Characters!.Results.Length == 0)
                    {
                        await context.Message.RespondAsync("No character found.");
                    }
                    else
                    {
                        var characterId = response!.Data.Characters.Results[0].Id;
                        var tserialized = JsonSerializer.Serialize(new
                        {
                            variables = new { id = characterId },
                            query = characterGraphQL
                        });

                        var tpayload = new StringContent(tserialized, Encoding.UTF8, "application/json");
                        var tfeed = await elfin.HttpClient.PostAsync("https://graphql.anilist.co/", tpayload);
                        var traw = await tfeed.Content.ReadAsStringAsync();
                        var tresponse = JsonSerializer.Deserialize<AniListResponse>(traw);
                        var character = tresponse!.Data.Character;
                        var imageData = character!.Image;
                        var siteUrl = character.SiteUrl;
                        var fullName = $"{character.Name.Full}";
                        var bloodType = (character.BloodType == "" ? "" : character.BloodType) ?? "N/A";
                        var author = new DiscordEmbedBuilder.EmbedAuthor()
                        {
                            Name = "AniList",
                            IconUrl = "https://i.imgur.com/iUIRC7v.png",
                            Url = "https://anilist.co"
                        };

                        var thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                        {
                            Url = character.Image.Large
                        };

                        var pages = new[] {
                            new Page("", new DiscordEmbedBuilder() {
                                Color = new DiscordColor("#2F3136"),
                                Author = author,
                                Url = siteUrl,
                                Title = fullName,
                                Thumbnail = thumbnail,
                                Description = $@"
                                    Id: `{character.Id}`
                                    Films: `{character.Media.Edges.Count}`
                                    Gender: `{character.Gender}`
                                    Age: `{(character.Age ?? "N/A")}`
                                    Favourites: `{character.Favourites}`
                                    Blood type: `{(bloodType)}`
                                "
                            }),
                            new Page("", new DiscordEmbedBuilder() {
                                Color = new DiscordColor("#2F3136"),
                                Author = author,
                                Thumbnail = thumbnail,
                                Url = siteUrl,
                                Title = fullName,
                                Description = $"{character.Description.Substring(0, 2043)}`...`"
                            })
                        };

                        var interactivity = elfin.RawClient.GetInteractivity();

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
                var query = HttpUtility.UrlEncode(string.Join(" ", context.Args));
                var feed = await elfin.HttpClient.GetAsync($"https://safebooru.org/index.php?page=dapi&s=post&q=index&json=1&tags={query}&limit=1000");
                var raw = await feed.Content.ReadAsStringAsync();

                if (raw == "")
                {
                    await context.Message.RespondAsync("No images found.");
                }
                else
                {
                    var response = JsonSerializer.Deserialize<List<SafeBooruResponse>>(raw);
                    var random = new Random();
                    var pick = response![random.Next(0, response.Count)];

                    await context.Message.RespondAsync($"https://safebooru.org/images/{pick.Directory}/{pick.Image}");
                }
            }
        }
    }
}