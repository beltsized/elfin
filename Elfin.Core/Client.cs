using Elfin.Types;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;

namespace Elfin.Core
{
    public class ElfinClient
    {
        public ElfinRegistrar Registrar { get; init; }
        public DiscordClient RawClient { get; init; }
        public HttpClient HttpClient { get; init; }
        public ElfinEvent[] Events = { };
        public ElfinCommand[] Commands = { };
        public string Prefix;

        public ElfinClient(ElfinData data)
        {
            this.Registrar = new ElfinRegistrar(this);
            this.RawClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = data.Token,
                TokenType = TokenType.Bot,
                Intents = data.Intents,
                MinimumLogLevel = data.LogLevel
            });

            this.HttpClient = new HttpClient();
            this.Prefix = data.Prefix;
        }

        public void DisableCommands()
        {
            foreach (ElfinCommand command in this.Commands)
            {
                command.Enabled = false;
            }
        }

        public void EnableCommands()
        {
            foreach (ElfinCommand command in this.Commands)
            {
                command.Enabled = true;
            }
        }

        public void LoadCommands()
        {
            this.Commands = this.Registrar.ReadCommands();
        }

        public void LoadEvents()
        {
            this.Events = this.Registrar.ReadEvents();

            foreach (ElfinEvent ev in this.Events)
            {
                ev.Initialize();
            }
        }

        public bool IsCompatible(string name, ElfinCommand command)
        {
            return name == command.Name || command.Aliases.Any(alias => name == alias);
        }

        public ElfinCommand? GetCommand(string name)
        {
            return this.Commands.FirstOrDefault(command => IsCompatible(name, command));
        }

        public void HandlePossibleCommand(MessageCreateEventArgs packet)
        {
            DiscordMessage message = packet.Message;
            string messageContent = message.Content;

            if (!message.Author.IsBot && messageContent.StartsWith(this.Prefix))
            {
                string[] components = messageContent.Split(" ");
                string commandName = components[0].Replace(this.Prefix, "").ToLower();
                ElfinCommand? command = this.GetCommand(commandName);

                if (command != null && command.Enabled)
                {
                    ElfinCommandContext context = new()
                    {
                        Packet = packet,
                        Author = packet.Author,
                        Guild = packet.Guild,
                        Channel = packet.Channel,
                        Message = message,
                        Args = components[1..]
                    };

                    command.Respond!(this, context);
                }
            }
        }

        public async Task Login()
        {
            await this.RawClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}