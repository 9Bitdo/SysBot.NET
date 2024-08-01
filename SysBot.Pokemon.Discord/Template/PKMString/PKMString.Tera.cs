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
    // URL
    public Task<string> TeraType => this.GetTeraType(pkm);
        
    
    private async Task<string> GetTypeIcon(byte TeraType, int TeraIconType, DiscordEmoji Emoji = null)
    {
        Bitmap? bitmap = null;
        // Pull the Gem type icons
        if ( TeraIconType == (int)DiscordSettings.EmbedSettingConfig.TeraTypeModeEnum.Gem )
        {
            bitmap = TypeSpriteUtil.GetTypeSpriteGem(TeraType);
            
        }
        // Pull the Square type icons
        else if ( TeraIconType == (int)DiscordSettings.EmbedSettingConfig.TeraTypeModeEnum.Square )
        {
            bitmap = TypeSpriteUtil.GetTypeSpriteIcon(TeraType, 9);
        }
        // Pull the SquareS type icons
        else if ( TeraIconType == (int)DiscordSettings.EmbedSettingConfig.TeraTypeModeEnum.SquareS )
        {
            bitmap = TypeSpriteUtil.GetTypeSpriteIconSmall(TeraType, 9);
        }
        // Pull the Wide type icons
        else if ( TeraIconType == (int)DiscordSettings.EmbedSettingConfig.TeraTypeModeEnum.Wide )
        {
            bitmap = TypeSpriteUtil.GetTypeSpriteWide(TeraType, 9);
        }

        // Generate an emoji instance
        string emojiName = $"type_icon_{(byte)TeraType:00}";
        if (Emoji == null)
        {
            Emoji = this.API.Emoji(emojiName);
        }
        else
        {
            Emoji.name = emojiName;
            Emoji.id = await Emoji.FindID();
        }
        
        // Upload the emoji if its not present in Discord
        if (Emoji.id == "")
            await Emoji.Upload(bitmap);

        return $"<:MoveEmoji:{Emoji.id}> ";
    }


    private async Task<string> GetTeraType(PKM pkm)
    {
        // Define variables
        Bitmap bitmap = null;

        // Return empty content if its not PK9 and exit the processes
        // pk refers to (PK9) pkm
        if ( !( pkm is PK9 pk) )
            return "";

        // Return text
        if ( Hub.Config.Discord.EmbedSetting.TeraTypeMode == DiscordSettings.EmbedSettingConfig.TeraTypeModeEnum.Text )
        {
            return $"{Strings.types[(byte)pk.TeraType]}";
        }
        else
        {
            return await GetTypeIcon( (byte)((PK9)pkm).TeraType, (int)Hub.Config.Discord.EmbedSetting.TeraTypeMode);
        }
        
    }
    
}
