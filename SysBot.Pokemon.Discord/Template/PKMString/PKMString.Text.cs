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
    // Definition of the message's properties
    public string Species => this.GetSpecies(pkm);
    public string Shiny => this.GetShiny(pkm);
    public string Scale => this.GetScale(pkm);
    public string Ability => this.GetAbility(pkm);
    public string Nature => this.GetNature(pkm);
    public List<int> IVs => this.GetIvs(pkm);
    
        
    // Definition of message's info/method
    private string GetSpecies(PKM pkm)
    {
        string specieName = $"{SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, (byte)(pkm.Generation <= 8 ? 8 : 9))}";
        string specieForm = TradeExtensions<T>.FormOutput(pkm.Species, pkm.Form, out _);
        string specieInfo = $"{specieName}{specieForm}";
        return specieInfo;
    }

    private string GetShiny(PKM pkm)
    {
        return pkm.ShinyXor == 0 ? "■" : pkm.ShinyXor <= 16 ? "★" : "";
    }
    private string GetAbility(PKM pkm)
    {
        return $"{this.Strings.Ability[pkm.Ability]}";
    }
    private string GetNature(PKM pkm)
    {
        return $"{ this.Strings.Natures[(int)pkm.Nature] }";
    }

    private List<int> GetIvs(PKM pkm)
    {
        return new() {pkm.IV_HP, pkm.IV_ATK, pkm.IV_DEF, pkm.IV_SPA, pkm.IV_SPD, pkm.IV_SPE};
    }
    
    private string GetScale(PKM pkm)
    {
        string scale = "";

        if (pkm is PK9 fin9)
            scale = $"{PokeSizeDetailedUtil.GetSizeRating(fin9.Scale)} ({fin9.Scale})";
        if (pkm is PA8 fin8a)
            scale = $"{PokeSizeDetailedUtil.GetSizeRating(fin8a.Scale)} ({fin8a.Scale})";
        if (pkm is PB8 fin8b)
            scale = $"{PokeSizeDetailedUtil.GetSizeRating(fin8b.HeightScalar)} ({fin8b.HeightScalar})";
        if (pkm is PK8 fin8)
            scale = $"{PokeSizeDetailedUtil.GetSizeRating(fin8.HeightScalar)} ({fin8.HeightScalar})";

        return scale;
    }

    
    
}
