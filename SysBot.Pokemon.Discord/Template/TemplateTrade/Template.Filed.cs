using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PKHeX.Core;
using SysBot.Base;

namespace SysBot.Pokemon.Discord;

public partial class TemplateTrade<T> where T : PKM, new()
{
    
    //
    private async Task PokemonInfoBasicFiled(bool inline)
    {
        // Obtain species's info
        string speciesInfo = pkmString.Species;
        // Obtain holditem's info
        string shiny = pkmString.Shiny;
        // Obtain Gender's info (Display Emoji if used, otherwise Gender)
        // LINQ C#: format is condition ? code when condition met : code when condition not met
        // LINQ C#: if GenderEmoji's option is enabled in the settings use GenderEmoji, otherwise use text Gender
        // DW 判断改在PKMString中进行，确保代码整洁
        string gender = await pkmString.Gender;
        // Obtain Mark's info
        (_, string markEntryText) = await pkmString.Mark;
        // Build info
        string filedName = $"{shiny} {speciesInfo} {gender} {markEntryText}";
        string filedValue = $"** **";
        embed.AddField(filedName, filedValue, inline);
    }

    private async Task HeldItemFiled(bool inline)
    {
        // Obtain holditem's info
        string heldItem = await pkmString.heldItem;
        if (heldItem == "")
            return ;
        
        string filedName = $"**Item Held:** {heldItem}";
        string filedValue = "** **";
        embed.AddField(filedName, filedValue, inline);
    }
    
    private async Task MarkFiled(bool inline)
    {
        // Obtain Mark's info
        (string mark, _) = await pkmString.Mark;
        if (mark == "")
            return;
        string filedName =  $"**Pokemon Mark:** {mark}";
        string filedValue = "** **";

        embed.AddField(filedName, filedValue, inline);
    }

    private async Task PokemonInfoFiled(bool inline)
    {
        // Obtain teraType's info
        string teraType = await pkmString.TeraType;
        // Define Level's info
        int level = pkm.CurrentLevel;
        // Define Ability's info
        string ability = pkmString.Ability;
        // Obtain Nature's Nature
        string nature = pkmString.Nature;
        // Obtain Scale's info
        string scale = pkmString.Scale;

        // Build info 
        var trademessage = "";
        // trademessage += pkm.Generation != 9 ? "" : useEmoji ? $"**Emoji:** {Emoji}\n" : $"**TeraType:** {teraType}\n";
        // LINQ C#: format is condition ? code when condition met : code when condition not met
        // LINQ C#: If pkm.generation is 9, check secondCondition, if secondCondition is true, result will be valueIfTrue otherwise
        // valueIfFalse if secondcondition is false, result will be empty string if pkm.generation is not 9
        // LINQ C#: if TeraTypeEmoji's option is enabled in the settings, use TeraTypeEmoji, otherwise use text 
        // The judgment is made in PKMString to ensure the cleanliness of the code
        trademessage += $"**Tera Type:** {await pkmString.TeraType}\n";
        // trademessage += pkm.Generation != 9 ? "" : Hub.Config.Discord.EmbedSetting.TeraTypeEmoji ? $"**Tera Type:** {pkmString.TeraTypeEmoji}\n" : $"**Tera Type:** {teraType}\n";
        trademessage += $"**Level:** {level}\n";
        trademessage += $"**Ability:** {ability}\n";
        trademessage += $"**Nature:** {nature}\n";
        trademessage += $"**Scale:** {scale}\n";
        
                
        // Build info
        string filedName = $"Pokémon Stats:";
        string filedValue = $"{trademessage}";

        embed.AddField(filedName, filedValue, inline);
    }

    private async Task MoveFiled(bool inline)
    {                
        string moveset = "";
        List<string> MovesEmoji = await pkmString.MovesEmoji;

        for (int i = 0; i < pkmString.Moves.Count; i++)
        {
            // Obtain Moveset
            string moveString = pkmString.Moves[i];
            // Obtain MovePP
            int movePP = i == 0 ? pkm.Move1_PP : i == 1 ? pkm.Move2_PP : i == 2 ? pkm.Move3_PP : pkm.Move4_PP;
            // If PP = 0, move will not be generated
            // Implement syntax 'break' for exiting out of the loop
            if (movePP == 0)
                break;
            // Setup moveEmoji
            string moveEmoji = MovesEmoji[i];
            // Generate Moveset's info
            moveset += $"- {moveEmoji}{moveString} ({movePP}PP)\n";
        }

        string FiledName = $"Moveset:";
        string FiledValue = moveset;

        embed.AddField(FiledName, FiledValue, inline);
    }

    private async Task IVsFiled(bool inline)
    {     
        string IVs = "";
        IVs += $"- {pkmString.IVs[0]} HP\n";
        IVs += $"- {pkmString.IVs[1]} ATK\n";
        IVs += $"- {pkmString.IVs[2]} DEF\n";
        IVs += $"- {pkmString.IVs[3]} SPA\n";
        IVs += $"- {pkmString.IVs[4]} SPD\n";
        IVs += $"- {pkmString.IVs[5]} SPE\n";

        string filedName = $"Pokémon IVs:";
        string filedValue = IVs;

        embed.AddField(filedName, filedValue, inline);        
            
    }

    private async Task EvsFiled(bool inline)
    {            
        string EVs = "";
        EVs += $"- {pkm.EV_HP} HP\n";
        EVs += $"- {pkm.EV_ATK} ATK\n";
        EVs += $"- {pkm.EV_DEF} DEF\n";
        EVs += $"- {pkm.EV_SPA} SPA\n";
        EVs += $"- {pkm.EV_SPD} SPD\n";
        EVs += $"- {pkm.EV_SPE} SPE\n";

        string filedName = $"Pokémon EVs:";
        string filedValue = EVs;
            
        embed.AddField(filedName, filedValue, inline);
    }

    private static async Task FiledTemp(bool inline)
    {                
        embed.AddField($"** **", $"** **", inline);
    }
   
}
