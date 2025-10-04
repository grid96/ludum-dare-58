using System;

public class StampModel
{
    public enum StampType
    {
        MonaLisa,
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
        StampType.MonaLisa => StampRarity.Legendary,
        _ => throw new ArgumentOutOfRangeException()
    };

    public float Condition { get; set; }
    
    public StampModel(StampType type, float condition) => (Type, Condition) = (type, condition);
}