using Elfin.Core;
using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinCommand
    {
        public required string Name { get; init; }
        public required string[] Aliases { get; init; }
        public required string Usage { get; init; }
        public required string Description { get; init; }
        public required Func<ElfinClient, ElfinCommandContext, Task> Respond { get; init; }
        public bool Enabled = true;

        public ElfinCommand()
        { }

        public ElfinCommand(ElfinCommand commandData)
        {
            this.Name = commandData.Name;
            this.Aliases = commandData.Aliases;
            this.Usage = commandData.Usage;
            this.Description = commandData.Description;
        }
    }
}