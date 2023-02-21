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
            return typeof(Program).Assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        public ElfinCommand[] ReadCommands()
        {
            List<ElfinCommand> commands = new List<ElfinCommand>();
            IEnumerable<Type> groupClasses = GetTypesWithAttribute<ElfinGroupAttribute>();
            IEnumerable<MethodInfo> groupMethods = groupClasses.SelectMany(t => t.GetMethods());

            foreach (MethodInfo method in groupMethods)
            {
                if (method.GetCustomAttribute(typeof(ElfinCommandAttribute)) == null)
                {
                    continue;
                }

                string commandName = "";
                string[] aliases = { };
                string usage = "";
                string description = "";
                IEnumerable<Attribute> attributes = method.GetCustomAttributes();

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
                        case ElfinUsageAttribute:
                            usage = ((ElfinUsageAttribute)attr).Usage;

                            break;
                        case ElfinDescriptionAttribute:
                            description = ((ElfinDescriptionAttribute)attr).Description;

                            break;
                    }
                }

                ElfinCommand newCommand = new()
                {
                    Name = commandName,
                    Aliases = aliases,
                    Usage = usage,
                    Description = description,
                    Respond = (ElfinClient elfin, ElfinCommandContext context) => method.Invoke(null, new object[] { elfin, context })
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