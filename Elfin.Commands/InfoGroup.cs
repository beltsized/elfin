using Elfin.Attributes;
using Elfin.Types;
using Elfin.Core;
using DSharpPlus.Entities;

namespace Elfin.Commands
{
    [ElfinGroup("info")]
    public class ElfinInfoCommandGroup
    {
        [ElfinCommand("help")]
        [ElfinAliases(new string[] { "commands", "helpme" })]
        [ElfinUsage("[command name]")]
        [ElfinDescription("Sends the command list.")]
        public static async Task Help(ElfinClient elfin, ElfinCommandContext context)
        {
            DiscordEmbedBuilder embed = new()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor(),
                Color = new DiscordColor("#2F3136"),
            };

            if (context.Args.Length == 0)
            {
                string[] commands = elfin.Commands.Select(c => $"`{elfin.Prefix}{c.Name}`").ToArray();

                embed.Author.Name = $"{commands.Length} commands";
                embed.Description = string.Join("\n", commands);
            }
            else
            {
                ElfinCommand? command = elfin.GetCommand(context.Args[0]);

                if (command == null)
                {
                    await context.Message.RespondAsync("No command found.");
                }
                else
                {
                    string[] aliases = command.Aliases.Select(a => $"`{a}`").ToArray();

                    embed.Author.Name = $"{elfin.Prefix}{command.Name}";
                    embed.Description = $@"
                        {command.Description}

                        Aliases: {string.Join(", ", aliases)}
                        Usage: `{command.Usage}`
                    ";
                }
            }

            await context.Message.RespondAsync(embed.Build());
        }

        [ElfinCommand("userinfo")]
        [ElfinAliases(new string[] { "uinfo", "uinf", "ui" })]
        [ElfinUsage("[user mention]")]
        [ElfinDescription("Sends information on any specified user.")]
        public static async Task UserInfo(ElfinClient elfin, ElfinCommandContext context)
        {
            var message = context.Message;
            var mentions = message.MentionedUsers;

            if (mentions.Count == 0)
            {
                await message.RespondAsync("You must mention a user first.");
            }
            else
            {
                var user = await context.Packet.Guild.GetMemberAsync(mentions[0].Id);
                var embed = new DiscordEmbedBuilder()
                {
                    Color = user.Color,
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = user.AvatarUrl
                    },
                    ImageUrl = user.BannerUrl,
                    Description = @$"
                        Handle: `{user.Username}#{user.Discriminator}`
                        Display: `{user.DisplayName}`
                        Muted: `{user.IsMuted}`
                        Boosting: `{user.PremiumSince != null}`
                        Joined: `{user.JoinedAt.ToString("MM/dd/yy")}`
                        Deafened: `{user.IsDeafened}`
                        Robotic: `{user.IsBot.ToString()}`
                        Systematic: `{(user.IsSystem != null ? "True" : "False")}`
                        Roles: `{user.Roles.ToArray().Length}`
                    "
                };

                await message.RespondAsync(embed.Build());
            }
        }
    }
}