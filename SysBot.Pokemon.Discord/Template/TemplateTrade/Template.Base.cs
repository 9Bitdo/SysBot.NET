using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PKHeX.Core;
using SysBot.Base;

namespace SysBot.Pokemon.Discord;

public partial class TemplateTrade<T>(PKM pkm, SocketCommandContext Context, PokeTradeHub<T> Hub) where T : PKM, new()
{
    private readonly PKM pkm = pkm;
    public readonly PKMString<T> pkmString = new(pkm, Hub);
    private readonly SocketCommandContext Context = Context;
    private readonly PokeTradeHub<T> Hub = Hub;
    private static EmbedBuilder embed = new();

    private Color SetColor()
    {
        return pkm.IsShiny && pkm.ShinyXor == 0 ? Color.Gold : pkm.IsShiny ? Color.LighterGrey : Color.Teal;
    }
    
    private EmbedAuthorBuilder SetAuthor()
    {
        EmbedAuthorBuilder author = new()
        {
            Name = $"{Context.User.Username}'s Pokémon",
            IconUrl = pkmString.ballImg
        };

        return author;
    }

    private async Task<string> SetThumbnailUrl()
    {
        return await pkmString.pokeImg;
    }

    private EmbedFooterBuilder SetFooter(int positionNum = 1, string etaMessage = "")
    {
        // Current queue position
        string Position = $"Current Position:{positionNum}";
        // Trainer info
        string Trainer = $"OT:{pkm.OriginalTrainerName} | TID:{pkm.DisplayTID} | SID:{pkm.DisplaySID}";

        // display combined footer content
        string FooterContent = "";
        FooterContent += $"\n{Position}";
        FooterContent += $"\n{Trainer}";
        FooterContent += $"\n{etaMessage}";

        return new EmbedFooterBuilder { Text = FooterContent };
    }


    public async Task<EmbedBuilder> Generate(int positionNum = 1, string etaMessage = "")
    {   
        // Build discord Embed
        embed = new EmbedBuilder { 
            Color = SetColor(), 
            Author = SetAuthor(), 
            Footer = SetFooter(), 
            ThumbnailUrl = await SetThumbnailUrl(),
            };
        
        // Build embed files        
        // SetFiled1
        await PokemonInfoBasicFiled(false);

        // SetFiled2
        await MarkFiled(false);
        
        // SetFiled3
        await HeldItemFiled(false);

        // SetFiled4
        await PokemonInfoFiled(true); // SetFiled4_1
        await FiledTemp(true); // SetFiled4_2
        await MoveFiled(true); // SetFiled4_3
        
        // SetFiled5
        await IVsFiled(true); // SetFiled5_1
        await FiledTemp(true); // SetFiled5_2
        await EvsFiled(true); // SetFiled5_3

        
        
        
        return embed;
    }
}
