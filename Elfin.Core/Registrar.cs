using Elfin.Attributes;
using Elfin.Types;
using System.Reflection;
using System.Linq;
using DSharpPlus.Entities;

namespace Elfin.Core
{
    public class ElfinRegistrar
    {
        public IEnumerable<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            return typeof(Program).Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        public ElfinCommand[] ReadCommands()
        {
            List<ElfinCommand> commands = new List<ElfinCommand>();
            IEnumerable<Type> groupClasses = GetTypesWithAttribute<ElfinGroupAttribute>();
            IEnumerable<MethodInfo> commandClasses = groupClasses.SelectMany(type => type.GetMethods());

            foreach (MethodInfo command in commandClasses)
            {
                string commandName = "";
                string[] aliases = { };
                IEnumerable<Attribute> attributes = command.GetCustomAttributes();

                foreach (Attribute attr in attributes)
                {
                    string? attrName = attr.ToString();

                    switch (attr)
                    {
                        case ElfinCommandAttribute:
                            commandName = ((ElfinCommandAttribute)attr).Name;

                            break;
                        case ElfinAliasesAttribute:
                            aliases = ((ElfinAliasesAttribute)attr).Aliases;
                            break;
                    }
                }

                ElfinCommand newCommand = new()
                {
                    Name = commandName,
                    Aliases = aliases,
                    Respond = (DiscordMessage message, string[] args) => command.Invoke(null, new object[] { message, args })
                };

                commands.Add(newCommand);
            }

            return commands.ToArray();
        }

        public ElfinEvent[] ReadEvents()
        {
            List<ElfinEvent> events = new List<ElfinEvent>();
            IEnumerable<Type> eventClasses = GetTypesWithAttribute<ElfinEventAttribute>();

            foreach (Type ev in eventClasses)
            {
                MethodInfo? response = ev.GetMethod("Respond");
                Attribute? attr = ev.GetCustomAttribute(typeof(ElfinEventAttribute));
                string eventName = ((ElfinEventAttribute)attr).Name;

                ElfinEvent newEvent = new()
                {
                    Name = eventName,
                    Respond = (DiscordMessage message, string[] args) => response.Invoke(null, new object[] { message, args })
                };

                events.Add(newEvent);
            }

            return events.ToArray();
        }
    }
}