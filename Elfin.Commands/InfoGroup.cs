using Elfin.Attributes;
using Elfin.Types;
using Elfin.Core;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Elfin.Commands
{
    [ElfinGroup("info")]
    public class ElfinInfoCommandGroup
    {
        [ElfinCommand("userinfo")]
        [ElfinAliases(new string[] { "uinfo", "uinf", "ui" })]
        [ElfinDescription("Sends information on any specified user.")]
        public static async Task UserInfo(ElfinClient elfin, ElfinCommandContext context)
        {
            DiscordMessage message = context.Packet.Message;
            IReadOnlyList<DiscordUser> mentions = message.MentionedUsers;

            if (mentions.Count == 0)
            {
                await message.RespondAsync("You must mention a user first.");
            }
            else
            {
                DiscordMember user = await context.Packet.Guild.GetMemberAsync(mentions[0].Id);
                DiscordEmbedBuilder embed = new()
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
                        Roles: `{user.Roles.ToArray().Length}`
                        Systematic: `{(user.IsSystem != null ? "True" : "False")}`
                    "
                };

                await message.RespondAsync(embed.Build());
            }
        }
    }
}