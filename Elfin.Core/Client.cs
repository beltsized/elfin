using Elfin.Types;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Elfin.Core
{
    public class ElfinClient
    {
        public DiscordClient? RawClient;
        public ElfinCommand[] Commands = { };
        public string Prefix;
        public ElfinRegistrar Registrar;

        public ElfinClient(ElfinData data)
        {
            this.Registrar = new ElfinRegistrar();
            this.RawClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = data.Token,
                TokenType = TokenType.Bot,
                Intents = data.Intents,
                MinimumLogLevel = data.LogLevel
            });

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

        public bool CheckIfCommandIsThisOne(string name, ElfinCommand command)
        {
            Console.WriteLine(name, command.Name, command.Aliases);
            if (name == command.Name)
            {
                return true;
            }
            else
            {
                foreach (string alias in command.Aliases)
                {
                    Console.WriteLine(alias);
                    if (name == alias)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public ElfinCommand? GetCommand(string name)
        {
            foreach (ElfinCommand command in this.Commands)
            {
                bool itsThisOne = this.CheckIfCommandIsThisOne(name, command);

                if (itsThisOne) {
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
                string commandName = components[0].Replace(this.Prefix, "").ToLower();
                ElfinCommand? command = this.GetCommand(commandName);

                if (command != null && command.Enabled)
                {
                    Console.WriteLine(true);
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