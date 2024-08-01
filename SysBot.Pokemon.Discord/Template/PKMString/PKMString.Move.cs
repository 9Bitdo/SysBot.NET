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


public partial class PKMString<T> where T : PKM, new()
{
    public List<string> Moves => this.GetMoves(pkm);
    public Task<List<string>> MovesEmoji => this.GetMovesEmoji(pkm);

    private List<string> GetMoves(PKM pkm)
    {
        List<string> Moves = new();
        
        for (int moveIndex = 0; moveIndex < pkm.Moves.Length; moveIndex++)
            Moves.Add(Strings.Move[ pkm.Moves[moveIndex] ]);
        
        return Moves;
    }

    private async Task<List<string>> GetMovesEmoji(PKM pkm)
    {
        if (Hub.Config.Discord.EmbedSetting.MoveTypeMode == DiscordSettings.EmbedSettingConfig.TeraTypeModeEnum.Text)
            return ["","","",""];

        // This judgement in the code can be skipped or removed because there are no further conditions or alternative paths
        // if (Hub.Config.Discord.EmbedSetting.MoveTypeMode == DiscordSettings.EmbedSettingConfig.MoveTypeModeEnum.Emoji)
        List<string> MovesEmoji = new();
        DiscordEmoji moveEmoji = this.API.Emoji("Marks");

        for (int moveIndex = 0; moveIndex < pkm.Moves.Length; moveIndex++)
        {
            byte moveTypeValue = MoveInfo.GetType(pkm.Moves[moveIndex], default);
            
            string moveEmojiCode = await GetTypeIcon( moveTypeValue, (int)Hub.Config.Discord.EmbedSetting.MoveTypeMode, moveEmoji);
            // string moveEmojiCode = await GetTypeIcon( moveTypeValue, (int)Hub.Config.Discord.EmbedSetting.MoveTypeMode);
            MovesEmoji.Add( moveEmojiCode );
        }
            
        return MovesEmoji;
    }

}
