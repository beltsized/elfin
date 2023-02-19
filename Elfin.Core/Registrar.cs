using Elfin.Attributes;
using Elfin.Types;
using System.Reflection;
using System.Linq;
using DSharpPlus.Entities;

namespace Elfin.Core
{
    public class ElfinRegistrar
    {
        public ElfinClient Client;

        public ElfinRegistrar(ElfinClient elfin)
        {
            this.Client = elfin;
        }

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
                    Respond = (ElfinClient elfin, ElfinCommandContext context) => command.Invoke(null, new object[] { elfin, context })
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
                MethodInfo? initializer = ev.GetMethod("Initalize");
                Attribute? attr = ev.GetCustomAttribute(typeof(ElfinEventAttribute));
                string eventName = ((ElfinEventAttribute)attr!).Name;

                ElfinEvent newEvent = new()
                {
                    Name = eventName,
                    Initialize = () => initializer!.Invoke(null, new object[] { this.Client })
                };

                events.Add(newEvent);
            }

            return events.ToArray();
        }
    }
}