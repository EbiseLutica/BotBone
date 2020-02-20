using System.Collections.Generic;
using System.Threading.Tasks;

using Disboard.Extensions;
using Disboard.Misskey.Models;

namespace Disboard.Misskey.Clients
{
    public class AdminClient : MisskeyApiClient
    {
        public Admin.EmojiClient Emoji { get; }
        protected internal AdminClient(MisskeyClient client) : base(client, "admin") 
        {
            Emoji = new Admin.EmojiClient(client);
        }
    }
}