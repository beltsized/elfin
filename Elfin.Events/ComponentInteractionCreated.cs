using Elfin.Attributes;
using Elfin.Core;
using DSharpPlus;

namespace Elfin.Events
{
    [ElfinEvent("ComponentInteractionCreated")]
    public class ComponentInteractionCreatedEvent
    {
        public static void Initalize(ElfinClient elfin)
        {
            elfin.RawClient.ComponentInteractionCreated += async (self, packet) =>
            {
                await packet.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            };
        }
    }
}