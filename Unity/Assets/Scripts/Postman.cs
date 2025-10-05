using System;
using UnityEngine;
using Random = System.Random;
using StampType = StampModel.StampType;

public class Postman : MonoBehaviour
{
    public static Postman Instance { get; private set; }

    [SerializeField] private Vector2[] envelopeSizes;
    [SerializeField] private Color[] envelopeColors;
    [SerializeField] private Color[] postmarkColors;

    private Random random = new();

    private void Awake() => Instance = this;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AddLetter();
    }

    public void OnMailboxClick()
    {
        for (int i = 0; i < random.Next(3, 7); i++)
            AddLetter();
    }
    
    public void AddLetter()
    { 
        var envelopeColor = envelopeColors[random.Next(0, envelopeColors.Length)];
        GameView.Instance.AddLetter(new LetterModel(envelopeSizes[random.Next(0, envelopeSizes.Length)], envelopeColor, "Recipient\nStreet\nCity\nCountry", "Sender\nStreet\nCity\nCountry", new StampModel(StampType.MonaLisa, (float)random.NextDouble() * 0.5f, (float)random.NextDouble() * 0.75f + 0.25f, new Vector2((float)random.NextDouble() * 0.3f - 0.15f, (float)random.NextDouble() * 0.3f - 0.15f), (float)random.NextDouble() * 60 - 30, postmarkColors[random.Next(0, postmarkColors.Length)], RandomDateUtil.RandomDateTimeExp(random), envelopeColor)));
    }
}