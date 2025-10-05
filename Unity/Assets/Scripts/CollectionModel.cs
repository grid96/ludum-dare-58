using System;
using System.Linq;
using UnityEngine;

public class CollectionModel
{
    public enum Collection
    {
        France,
        Painting,
        Postmark,
        Rare,
        Epic,
        Legendary,
    }

    public Collection Type { get; set; }
    public StampModel[] Stamps = new StampModel[4];
    public int[] Scores = new int[4];
    public int TotalScore => Scores.Sum();

    public CollectionModel(Collection type) => Type = type;

    public void CalculateScores()
    {
        for (int i = 0; i < Stamps.Length; i++)
            if (Stamps[i] != null && Type switch
                {
                    Collection.France => Stamps[i].Country == StampModel.StampCountry.France,
                    Collection.Painting => Stamps[i].Category == StampModel.StampCategory.Painting,
                    Collection.Postmark => Stamps[i].Postmark > 0,
                    Collection.Rare => Stamps[i].Rarity == StampModel.StampRarity.Rare,
                    Collection.Epic => Stamps[i].Rarity == StampModel.StampRarity.Epic,
                    Collection.Legendary => Stamps[i].Rarity == StampModel.StampRarity.Legendary,
                    _ => throw new ArgumentOutOfRangeException()
                } && Stamps.ToList().FindIndex(s => s?.Type == Stamps[i].Type) == i)
                Scores[i] = Type switch
                {
                    Collection.Postmark => ToScore((int)Stamps[i].Rarity / 2f, Stamps[i].Postmark * (DateTime.Now.Year - Stamps[i].PostmarkDate.Year) / 100f, 1 - Stamps[i].CutDeviation),
                    _ => ToScore((int)Stamps[i].Rarity / 2f, 1 - Stamps[i].Postmark - Stamps[i].Damage, 1 - Stamps[i].CutDeviation)
                };
            else
                Scores[i] = 0;
    }

    public int ToScore(float bonus, params float[] values) => Mathf.Clamp(Mathf.RoundToInt(values.Sum(v => Mathf.Clamp01(v) * 10) / values.Length + bonus), 0, 10);
}