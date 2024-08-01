using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PKHeX.Core;


namespace SysBot.Pokemon;

public partial class DiscordSettings
{
    // must be set to ExpandableObjectConverter class，otherwise "setting" panel will not recognize reason being:
    // The original EmbedSettingConfig, is a class object type for transferring between codes and
    // "setting" by itself cannot directly support class object types, so it must be converted to a type in order to be recognized.
    // Note: if you attempt to complie the line [TypeConverter(typeof(ExpandableObjectConverter))], EmbedSettingConfig's option will become unusable
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EmbedSettingConfig
    {
        // Implement optional mode to Emoji thus skipping the need to manually input EmojiCode
        // Enum is a data type, 0 = Sysbot mode and 1 = PKHeX mode
        // Set the data type of SpeciePicMode to the SpeciePicModeEnum object created to achieve the effect of the optional box
        // The content of the optional box depends on the content of Enum which can be rewrote
        [Description("Choose SysBot or PKHeX for different sprites display in the embed"),DisplayName("SpritePicMode")]
        public SpeciePicModeEnum SpeciePicMode { get; set; } = SpeciePicModeEnum.Sysbot;      
        public enum SpeciePicModeEnum{Sysbot, PKHeX}      

        [Description("Select Text, Emoji or upload custom emojis to Discord with filename, Male = gender_0, Female = gender_1 and Unknown = gender_2.")]
        public GenderModeEnum GenderMode { get; set; } = GenderModeEnum.Text;      
        public enum GenderModeEnum{Text, Emoji}  

        [Description("Select Gem, Square, SquareS or Wide on the first use, and delete from the serve before changing to another selection in the HUB setting. Custom Tera's emojis upload to serve must be saved under filename format: 'type_icon_00..01..02.\r\n")]
        public TeraTypeModeEnum TeraTypeMode { get; set; } = TeraTypeModeEnum.Text;      
        public enum TeraTypeModeEnum{Text, Gem, Square, SquareS, Wide}  
        
        [Description("MoveType's option must match TeraType; ie Gem = Gem")]
        public TeraTypeModeEnum MoveTypeMode { get; set; } = TeraTypeModeEnum.Text;      

        [Description("Select BigItems or ArtworkItems for held item sprites from PKHeX's database or Text for text form.\r\n")]
        public ItemEmojiTypeEnum ItemEmojiMode { get; set; } = ItemEmojiTypeEnum.Text;

        public enum ItemEmojiTypeEnum{Text, ArtworkItems, BigItems}
        
        [Description("Select Emoji for Pokémon's Mark/s sprites in the embed or Text in text form.")]
        public MarkEmojiModeEnum MarkEmojiMode { get; set; } = MarkEmojiModeEnum.Text;

        public enum MarkEmojiModeEnum{Text, Emoji}
        
        
        // public IEnumerator<MoveEmojiConfig> GetEnumerator() => MoveEmojiConfigs.GetEnumerator();
        // public IEnumerable<string> Summarize() => MoveEmojiConfigs.Select(z => z.ToString());
        public override string ToString() => "Discord Embed Integration Settings";        
    }
  
    

}
