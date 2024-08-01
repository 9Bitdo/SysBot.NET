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
using System.Collections.Concurrent;

namespace SysBot.Pokemon.Discord;


public partial class PKMString<T> where T : PKM, new()
{
    public Task<(string,string)> Mark => this.GetMark(pkm);
    
    private async  Task<(string,string)> GetMark(PKM pkm)
    {
        // Define variables
        string markEntryText = ""; 
        string Mark = "";
        string markEmoji = "";
        ConcurrentBag<RibbonIndex> ribbonIndexList = new ConcurrentBag<RibbonIndex>(); // Utitlising thread-safe collection 

        // Obtain the mark/s from the pokemon
        // Search the mark in Pokemon SV and add the pokemon to ribbonIndexList if found
        // Utitlise the computer's multi-core capability to achieve multiple tasks being carried out
        Parallel.For((int)RibbonIndex.MarkLunchtime, (int)RibbonIndex.Partner + 1, mark =>
        {
            if (((PK9)pkm).GetRibbon(mark))
            {
                ribbonIndexList.Add((RibbonIndex)mark);
            }
        });

        // Parallel.For Equivalent to the multi-threaded version for loop, the following is the original for loop code
        // for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.Partner; mark++)
        // {
        //     if (((PK9)pkm).GetRibbon((int)mark))
        //     {
        //         ribbonIndexList.Add(mark);
        //     }
        // }


        // If TEXT mode is selected in HUB
        if ( Hub.Config.Discord.EmbedSetting.MarkEmojiMode == DiscordSettings.EmbedSettingConfig.MarkEmojiModeEnum.Text)
        {
            // Utilising linq syntax to obtain information about each mark
            // Utilising strings to convert mark into the corresponding language
            var temp = ribbonIndexList.Select(x => $"{this.Strings.ribbons[(int)x]}{Environment.NewLine}");
            // Each mark is seperated by a comma(,)
            Mark = string.Join(",", temp);
            return (Mark, markEntryText);
        }

        // If EMOJI mode is selected in HUB
        if ( Hub.Config.Discord.EmbedSetting.MarkEmojiMode == DiscordSettings.EmbedSettingConfig.MarkEmojiModeEnum.Emoji)
        {
            // Instantiate Emoji
            var marksEmoji = this.API.Emoji("Marks");

            // Utilising 'WhenAll' allow async to run simultaneously
            // 'Select' is used instead of 'For' for loop to give the loop body an anonymous function to match 'WhenAll'
            await Task.WhenAll(ribbonIndexList.Select(async mark =>
            {
                // Build emojiName
                string emojiName = mark.ToString();

                // Obtain mark's image
                Bitmap bitmap = RibbonSpriteUtil.GetRibbonSprite(mark);

                // Modify the attributes of marksEmoji
                marksEmoji.name = emojiName;
                marksEmoji.id = await marksEmoji.FindID();
                
                // Save the commonly used MarkEmoji to Discord
                if ( (mark >= RibbonIndex.Hisui) && (mark <= RibbonIndex.Partner) )
                {
                    // Upload the emoji if it does not exist on Discord
                    if ( marksEmoji.id == "")
                        await marksEmoji.Upload(bitmap);
                }


                // Temporary save the MarkEmoji on Discord
                if ( (mark >= RibbonIndex.MarkLunchtime) && (mark <= RibbonIndex.MarkSlump) )
                {
                    // Upload the emoji if it does not exist on Discord
                    await marksEmoji.Upload(bitmap);
                    // Add emoji to cache for deletion
                    this.EmojiCache.Add(marksEmoji);
                }

                // Generate the corresponding Emoji code and add to MarkEmoji
                markEmoji += $"<:MarkEmoji:{marksEmoji.id}> ";
            } ));

            return (markEmoji, markEntryText);
        }

        return ("","");
    }
}
