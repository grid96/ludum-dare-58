using System;
using UnityEngine;

public class StampModel
{
    public enum StampType
    {
        EiffelTower,
        NotreDame,
        Louvre,
        SacreCoeur,
        MonaLisa,
        StarryNight,
        TheScream,
        GirlWithAPearlEarring,
        GreatWallOfChina,
        PyramidsOfGiza,
        Colosseum,
        TajMahal,
        MountFuji,
        BigBen,
        Sphinx,
        Pagoda,
        Parthenon,
        MachuPicchu,
        MoaiStatue,
        SydneyOperaHouse,
        ChichenItza,
        GoldenGateBridge,
        Stonehenge,
        AngkorWat,
        TheaterMasks,
        Violin,
        Drum,
        DancerSilhouette,
        FilmCamera,
        Crown,
        Torch,
        Book,
        Scroll,
        MaskOfTutankhamun,
        SushiRoll,
        PizzaSlice,
        Pretzel,
        TeaCup,
        IceCreamCone,
        CornCob,
    }

    public enum StampSize
    {
        Standard,
    }

    public enum StampCategory
    {
        Other,
        Painting,
    }

    public enum StampCountry
    {
        Other,
        France,
    }

    public enum StampRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
    }

    public StampType Type { get; set; }

    public StampSize Size => Type switch
    {
        _ => StampSize.Standard,
    };

    public StampCategory Category => Type switch
    {
        StampType.GirlWithAPearlEarring => StampCategory.Painting,
        StampType.TheScream => StampCategory.Painting,
        StampType.StarryNight => StampCategory.Painting,
        StampType.MonaLisa => StampCategory.Painting,
        _ => StampCategory.Other
    };

    public StampCountry Country => Type switch
    {
        StampType.EiffelTower => StampCountry.France,
        StampType.NotreDame => StampCountry.France,
        StampType.Louvre => StampCountry.France,
        StampType.SacreCoeur => StampCountry.France,
        _ => StampCountry.Other
    };

    public StampRarity Rarity => Type switch
    {
        StampType.SacreCoeur => StampRarity.Rare,
        StampType.NotreDame => StampRarity.Epic,
        StampType.MonaLisa => StampRarity.Legendary,
        StampType.StarryNight => StampRarity.Rare,
        StampType.SydneyOperaHouse => StampRarity.Legendary,
        StampType.TajMahal => StampRarity.Legendary,
        StampType.Crown => StampRarity.Legendary,
        StampType.SushiRoll => StampRarity.Epic,
        StampType.TeaCup => StampRarity.Epic,
        StampType.Torch => StampRarity.Epic,
        StampType.Pagoda => StampRarity.Epic,
        StampType.Colosseum => StampRarity.Epic,
        StampType.Book => StampRarity.Rare,
        StampType.MaskOfTutankhamun => StampRarity.Rare,
        StampType.DancerSilhouette => StampRarity.Rare,
        StampType.TheaterMasks => StampRarity.Rare,
        StampType.Violin => StampRarity.Rare,
        StampType.Parthenon => StampRarity.Rare,
        _ => StampRarity.Common
    };

    public float Damage { get; set; }
    public float Postmark { get; set; }
    public Vector2 PostmarkOffset { get; set; }
    public float PostmarkRotation { get; set; }
    public Color PostmarkColor { get; set; }
    public DateTime PostmarkDate { get; set; }
    public Color BackgroundColor { get; set; }
    public float CutDeviation { get; set; }

    public StampModel(StampType type, float damage, float postmark, Vector2 postmarkOffset, float postmarkRotation, Color postmarkColor, DateTime postmarkDate, Color backgroundColor) => (Type, Damage, Postmark, PostmarkOffset, PostmarkRotation, PostmarkColor, PostmarkDate, BackgroundColor) = (type, damage, postmark, postmarkOffset, postmarkRotation, postmarkColor, postmarkDate, backgroundColor);
}