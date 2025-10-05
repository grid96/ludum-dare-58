using System;
using UnityEngine;

public class StampModel
{
    public enum Collection
    {
        Category,
        Country,
        Rarity,
        Postmark,
    }

    public enum StampType
    {
        MonaLisa,
    }

    public enum StampSize
    {
        Standard,
    }

    public enum StampCategory
    {
        Painting,
    }

    public enum StampCountry
    {
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
        StampType.MonaLisa => StampCategory.Painting,
        _ => throw new ArgumentOutOfRangeException()
    };

    public StampCountry Country => Type switch
    {
        StampType.MonaLisa => StampCountry.France,
        _ => throw new ArgumentOutOfRangeException()
    };

    public StampRarity Rarity => Type switch
    {
        // StampType.MonaLisa => StampRarity.Legendary,
        _ => StampRarity.Common
    };

    public float Damage { get; set; }
    public float Postmark { get; set; }
    public Vector2 PostmarkOffset { get; set; }
    public float PostmarkRotation { get; set; }
    public Color PostmarkColor { get; set; }
    public DateTime PostmarkDate { get; set; }
    public Color BackgroundColor { get; set; }

    public StampModel(StampType type, float damage, float postmark, Vector2 postmarkOffset, float postmarkRotation, Color postmarkColor, DateTime postmarkDate, Color backgroundColor) => (Type, Damage, Postmark, PostmarkOffset, PostmarkRotation, PostmarkColor, PostmarkDate, BackgroundColor) = (type, damage, postmark, postmarkOffset, postmarkRotation, postmarkColor, postmarkDate, backgroundColor);

    public float GetConditionForCollection(Collection collection) => collection switch
    {
        Collection.Postmark => Postmark - Damage,
        _ => 1 - Damage
    };
}