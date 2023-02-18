using Elfin.Types;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace Elfin.Core
{
    public class ElfinClient
    {
        public DiscordClient? RawClient;
        public ElfinCommand[] Commands = { };
        public string Prefix;

        public ElfinClient(ElfinClientData ClientData)
        {
            this.RawClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = ClientData.Token,
                TokenType = TokenType.Bot,
                Intents = ClientData.Intents,
                MinimumLogLevel = ClientData.LogLevel
            });

            this.Prefix = ClientData.Prefix;
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

        public void SetCommands(ElfinCommand[] NewCommands)
        {
            this.Commands = NewCommands;
        }

        public ElfinCommand? GetCommand(string commandName)
        {
            foreach (ElfinCommand command in this.Commands)
            {
                if (command.Name == commandName)
                {
                    return command;
                }
            }

            return null;
        }

        public string[] ConvertToArgs(string[] components)
        {
            List<string> args = new List<string>(components);

            args.RemoveAt(0);

            return args.ToArray();
        }

        public void HandlePossibleCommand(MessageCreateEventArgs packet)
        {
            DiscordMessage message = packet.Message;
            string messageContent = message.Content;

            if (!message.Author.IsBot && messageContent.StartsWith(this.Prefix))
            {
                string[] components = messageContent.Split(" ");
                string commandName = components[0].Replace(this.Prefix, "");
                ElfinCommand? command = this.GetCommand(commandName);

                if (command != null && command.Enabled)
                {
                    string[] args = this.ConvertToArgs(components);

                    command.Respond(message, args);
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