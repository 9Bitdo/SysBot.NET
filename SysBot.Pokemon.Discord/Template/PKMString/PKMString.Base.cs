using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PKHeX;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PKHeX.Drawing.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord;


// Partial is used to separate different aspects of a class into multiple files to write to achieve readability and maintainability
// Partial class with the same name such as PKMString, is defined in different files within the same namespace hnnce treated as part of the same class
public partial class PKMString<T> where T : PKM, new()
{
    // Declare global API object
    private MyAPI API = null;
    
    // Default language
    private PKM pkm = null;
    private int PKMLanguage => pkm.Language;
    private int GameLanguage => PKMLanguage < 6 ? PKMLanguage - 1 : PKMLanguage == 6 || PKMLanguage == 7 ? 0 : PKMLanguage > 7 ? PKMLanguage - 2 : 0;
    private GameStrings Strings => GameInfo.GetStrings(GameLanguage);

    // Data panel
    private PokeTradeHub<T> Hub;

    // Cache list, emoji to be deleted are stored in
    public List<DiscordEmoji> EmojiCache = new();

    // Implement a method to delete emoji (trash temporary emoji cache)
    public async Task ClearCache()
    {
        foreach (DiscordEmoji emoji in this.EmojiCache)
        {
            await emoji.Delete();
        }
        
    }

    // Build function
    public PKMString(PKM pkm, PokeTradeHub<T> Hub)
    {
        this.pkm = pkm;
        this.Hub = Hub;

        // Instantiate the global MyAPI
        string channel = Hub.Config.Discord.EmojiChannel;
        string authorization = Hub.Config.Discord.Authorization;
        this.API = new MyAPI(channel, authorization);
    }

}
