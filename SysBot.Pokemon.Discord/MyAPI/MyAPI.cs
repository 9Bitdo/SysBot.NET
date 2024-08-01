using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SysBot.Pokemon.Discord;

// This class classifield all APIs equivalent to a directory, and to facilitate quick access to the corresponding APIs.
// Reduce the number of incoming parameters and ensure they are more reasonable.
public class MyAPI
{
    public string channel;
    public string token;

    public MyAPI(string channel, string token)
    {
        this.channel = channel;
        this.token = token;
    }

    public DiscordEmoji Emoji(string name) => new DiscordEmoji(name, this.channel, this.token);
}
