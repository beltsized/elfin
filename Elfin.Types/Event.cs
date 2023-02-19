using DSharpPlus.Entities;

namespace Elfin.Types
{
    public class ElfinEvent
    {
        public required string Name { get; init; }
        public required Action Initialize { get; init; }

        public ElfinEvent()
        { }

        public ElfinEvent(ElfinEvent eventData)
        {
            this.Name = eventData.Name;
            this.Initialize = eventData.Initialize;
        }
    }
}