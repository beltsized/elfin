using Elfin.Attributes;
using Elfin.Core;

namespace Elfin.Events
{
    [ElfinEvent("Ready")]
    public class ReadyEvent
    {
        public static void Initalize(ElfinClient elfin)
        {
            elfin.RawClient.Ready += async (self, packet) =>
            {
                Console.WriteLine("Elfin Is Ready!");
            };
        }
    }
}