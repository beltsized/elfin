using Elfin.Attributes;
using Elfin.Types;
using System.Reflection;

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
            var commands = new List<ElfinCommand>();
            var groupClasses = GetTypesWithAttribute<ElfinGroupAttribute>();
            var groupMethods = groupClasses.SelectMany(t => t.GetMethods());

            foreach (var method in groupMethods)
            {
                if (method.GetCustomAttribute(typeof(ElfinCommandAttribute)) == null)
                {
                    continue;
                }

                var commandName = "";
                var aliases = new string[] { };
                var usage = "";
                var description = "";
                var attributes = method.GetCustomAttributes();

                foreach (var attr in attributes)
                {
                    var attrName = attr.ToString();

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

                var newCommand = new ElfinCommand()
                {
                    Name = commandName,
                    Aliases = aliases,
                    Usage = usage,
                    Description = description,
                    Respond = (ElfinClient elfin, ElfinCommandContext context) =>
                    {
                        method.Invoke(null, new object[] { elfin, context });
                    }
                };

                commands.Add(newCommand);
            }

            return commands.ToArray();
        }

        public ElfinEvent[] ReadEvents()
        {
            var events = new List<ElfinEvent>();
            var eventClasses = GetTypesWithAttribute<ElfinEventAttribute>();

            foreach (Type ev in eventClasses)
            {
                var initializer = ev.GetMethod("Initalize");
                var attr = ev.GetCustomAttribute(typeof(ElfinEventAttribute));
                var eventName = ((ElfinEventAttribute)attr!).Name;

                var newEvent = new ElfinEvent()
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