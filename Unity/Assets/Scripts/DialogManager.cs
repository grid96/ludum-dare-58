using System;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class DialogManager
{
    [SerializeField] private RuntimeAnimatorController avatar;

    private static readonly string[] conditions = { "Poor", "Fair", "Good", "Very Good", "Excellent", "Mint" };
    private static readonly string[] cut = {"Terrible", "Very Bad", "Bad", "Decent", "Good", "Very Good", "Excellent", "Perfect" };
    private static readonly string[] postmark = {"Fully Removed", "Mostly Removed", "Barely visible", "Partially visible", "Mostly visible", "Fully visible" };

    public async UniTask ShowStampInfo(StampModel model)
    {
        string message = $"Stamp: {Format(model.Type)}{(model.Country == StampModel.StampCountry.France ? " - Origin: France" : "")} - Rarity: {Format(model.Rarity)} - Condition: {conditions[Mathf.Clamp(Mathf.RoundToInt((1 - model.Damage) * (conditions.Length - 1)), 0, conditions.Length - 1)]} - Cut: {cut[Mathf.Clamp(Mathf.RoundToInt((1 - model.CutDeviation) * (cut.Length - 1)), 0, cut.Length - 1)]} - Postmark: {postmark[Mathf.Clamp(Mathf.RoundToInt(model.Postmark * (postmark.Length - 1)), 0, postmark.Length - 1)] + (Mathf.Clamp(Mathf.RoundToInt(model.Postmark * (postmark.Length - 1)), 0, postmark.Length - 1) > 0 ? $" ({model.PostmarkDate.Year})" : "")}";
        
        await ShowMessage(avatar, message);
        await ClickToContinue();
        await Hide();
    }

    public async UniTask TestDialog()
    {
        /*await ShowMessage(avatar, "Hello World!");
        await ClickToContinue();
        await ShowMessage(avatar, "This is a test dialog.");
        await ClickToContinue();
        await ShowMessage(avatar, "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.");
        await ClickToContinue();
        await ShowMessage(avatar, "At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.");
        await ClickToContinue();
        await ShowMessage(avatar, "This is the last message.");
        await ClickToContinue();
        await ShowMessage(avatar, "Goodbye!");
        await ClickToContinue();
        await Hide();*/
    }

    public static string Format(Enum text)
    {
        if (string.IsNullOrEmpty(text.ToString()))
            return text.ToString();
        string spaced = Regex.Replace(text.ToString(), @"(?<=[a-z])([A-Z])", " $1");
        spaced = Regex.Replace(spaced, @"(?<=[A-Z])([A-Z][a-z])", " $1");
        if (char.IsLower(spaced[0]))
            spaced = char.ToUpper(spaced[0]) + spaced.Substring(1);
        return spaced.Trim();
    }
}