using Elfin.Types;

namespace Elfin.Core
{
    public class ElfinClient
    {
        public ElfinCommand[] Commands = {
            new ElfinCommand() {
                Name = "ping",
                Respond = async (Message) => {
                    await Message.RespondAsync("Pong!");
                }
            }
        };
    }
}