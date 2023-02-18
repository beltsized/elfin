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
            foreach (Type type in typeof(Program).Assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public ElfinCommand[] ReadCommands()
        {
            List<ElfinCommand> commands = new List<ElfinCommand>();

            var progGroupClasses = GetTypesWithAttribute<ElfinGroupAttribute>();

            Console.WriteLine(progGroupClasses.ToArray().Length);
            foreach (Type type in progGroupClasses)
            {
                foreach (MethodInfo tMethod in type.GetMethods()) {
                    Console.WriteLine(tMethod);
                }

                MethodInfo[] methods = type.GetMethods();

                Console.WriteLine(methods.Length);

                foreach (MethodInfo typeMethod in methods)
                {
                    string commandName = "";
                    string[] aliases = { };
                    IEnumerable<Attribute> attributes = typeMethod.GetCustomAttributes();

                    foreach (Attribute attr in attributes)
                    {
                        string? attrName = attr.ToString();

                        if (attrName.Contains("Command"))
                        {
                            commandName = ((ElfinCommandAttribute)attr).Name;
                        }
                        else if (attrName.Contains("Aliases"))
                        {
                            aliases = ((ElfinAliasesAttribute)attr).Aliases;
                        }
                    }

                    ElfinCommand newCommand = new()
                    {
                        Name = commandName,
                        Aliases = aliases
                    };

                    newCommand.Respond = (DiscordMessage message, string[] args) =>
                    {
                        Console.WriteLine(111111);
                        typeMethod.Invoke(newCommand, new object[] { message, args });
                    };

                    commands.Add(newCommand);
                }
            }

            return commands.ToArray();
        }

        public void ReadEvents()
        {

        }
    }
}