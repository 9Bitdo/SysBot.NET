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
using SysBot.Base;

namespace SysBot.Pokemon.Discord;


public partial class PKMString<T> where T : PKM, new()
{    
    // URL
    public string ballImg => this.GetBallImg(pkm);
    // Emoji that require deletion
    public Task<string> pokeImg => this.GetPokeImg(pkm);
    public Task<string> heldItem => this.GetHeldItem(pkm);
    // Emoji that does not require deletion
    public Task<string> Gender => this.GetGender(pkm);
        
    private string GetBallImg(PKM pkm)
    {
        return $"https://raw.githubusercontent.com/BakaKaito/HomeImages/main/Ballimg/50x50/" + $"{(Ball)pkm.Ball}ball".ToLower() + ".png";
    }

    private async Task<string> GetPokeImg(PKM pkm)
    {
        // Read the settings for SpeciePicMode in Hub and pull the type related image URL for SysBot mode
        if (Hub.Config.Discord.EmbedSetting.SpeciePicMode == DiscordSettings.EmbedSettingConfig.SpeciePicModeEnum.Sysbot)
        {
            return TradeExtensions<T>.PokeImg(pkm, false, false);
        }

        // Read the settings for SpeciePicMode in Hub and pull the type related image URL for PKHeX mode
        else if (Hub.Config.Discord.EmbedSetting.SpeciePicMode == DiscordSettings.EmbedSettingConfig.SpeciePicModeEnum.PKHeX)
        {
            // PID is added to name, to avoid species with the same name.
            string emojiName =  $"species" + pkm.PID;
            // Extract images from PkHex.Drawing.PokeSprite
            Bitmap bitmap =  SpriteUtil.GetSprite(pkm.Species, pkm.Form, pkm.Gender,((PK9)pkm).FormArgument, pkm.HeldItem, pkm.IsEgg, (Shiny)(pkm.IsShiny ? 2 : 0));
            // The integration of MyAPI, eliminate the need enter the channel, token, and emojiName concurrently
            // Channel and token have been presetted, hence only emojiName required to be passed in when instantiating 
            // The this.pokeEmoji, is a pre-defined global emoji and is not fixed, hence will be deleted in ClearCache to free up space
            var Emoji = this.API.Emoji(emojiName);
            // Add emoji to cache to be deleted later
            this.EmojiCache.Add(Emoji);
            // Upload the image 
            await Emoji.Upload(bitmap);
            // Obtain the URL after upload
            return Emoji.url;
        }
        return "";
    }

    public async Task<string> GetHeldItem(PKM pkm)
    {
        Bitmap? bitmap = null;

        // In PKHeX mode selection
        if (Hub.Config.Discord.EmbedSetting.SpeciePicMode == DiscordSettings.EmbedSettingConfig.SpeciePicModeEnum.PKHeX)
        {
            return "";
        }

        // If item does not exist
        if (pkm.HeldItem == 0)
        {
            return " - ";
        }

        // In Text mode selection
        if (Hub.Config.Discord.EmbedSetting.ItemEmojiMode == DiscordSettings.EmbedSettingConfig.ItemEmojiTypeEnum.Text)
        {
            string heldItem = pkm.HeldItem != 0 ? this.Strings.Item[pkm.HeldItem]: "";
            return heldItem;
        }

        // In Emoji mode selection
        // Instantiate emoji
        string item = $"item_{pkm.HeldItem}";
        string emojiName = item + pkm.PID;
        // The this.itemEmoji, is a pre-defined global emoji and is not fixed, hence will be deleted in ClearCache to free up space
        var Emoji = this.API.Emoji(emojiName);
        // Add Emoji to cache and delete them later
        this.EmojiCache.Add(Emoji);

        // Pull the image directly from the resource library of PKHeX.Drawing.PokeSprite

        if (Hub.Config.Discord.EmbedSetting.ItemEmojiMode == DiscordSettings.EmbedSettingConfig.ItemEmojiTypeEnum.ArtworkItems)
        {
            bitmap = PKHeX.Drawing.PokeSprite.Properties.Resources.ResourceManager.GetObject($"aitem_{pkm.HeldItem}") as Bitmap;
        }
        else if (Hub.Config.Discord.EmbedSetting.ItemEmojiMode == DiscordSettings.EmbedSettingConfig.ItemEmojiTypeEnum.BigItems)
        {
            bitmap = PKHeX.Drawing.PokeSprite.Properties.Resources.ResourceManager.GetObject($"bitem_{pkm.HeldItem}") as Bitmap;
        }

        // Upload emoji
        await Emoji.Upload(bitmap);

        // Double judgement
        // If ID does not exist, return empty content
        if (Emoji.id == "")
            return "";
        else
            return $"<:ItemEmoji:{Emoji.id}> ";
    }

    private async Task<string> GetGender(PKM pkm)
    {
        Bitmap bitmap = null;

        if (Hub.Config.Discord.EmbedSetting.GenderMode == DiscordSettings.EmbedSettingConfig.GenderModeEnum.Text)
            return pkm.Gender == 0 ? " - (M)" : pkm.Gender == 1 ? " - (F)" : "";
        
        if (Hub.Config.Discord.EmbedSetting.GenderMode == DiscordSettings.EmbedSettingConfig.GenderModeEnum.Emoji)
        {
            string emojiName = $"gender_{pkm.Gender}";
            DiscordEmoji Emoji = this.API.Emoji(emojiName);
            if (Emoji.id == "")
            {
                if (pkm.Gender == 0)
                    bitmap = PKHeX.WinForms.Properties.Resources.gender_0;
                else if (pkm.Gender == 1)
                    bitmap = PKHeX.WinForms.Properties.Resources.gender_1;
                else if (pkm.Gender == 2)
                    bitmap = PKHeX.WinForms.Properties.Resources.gender_2;
                else
                    bitmap = PKHeX.WinForms.Properties.Resources.gender_2;

                await Emoji.Upload(bitmap);
            }
            return $"<:GenderEmoji:{Emoji.id}> ";
        }
        return "";
    }

    
}
