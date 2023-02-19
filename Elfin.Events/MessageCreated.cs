using Elfin.Attributes;
using Elfin.Core;

namespace Elfin.Events
{
    [ElfinEvent("MessageCreated")]
    public class MessageCreatedEvent
    {
        public static void Initalize(ElfinClient elfin)
        {
            elfin.RawClient.MessageCreated += async (self, packet) =>
            {
                elfin.HandlePossibleCommand(packet);
            };
        }
    }
}