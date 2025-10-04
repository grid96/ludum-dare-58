using System;
using UnityEngine;
using Random = System.Random;
using StampType = StampModel.StampType;

public class Postman : MonoBehaviour
{
    public static Postman Instance { get; private set; }

    private Random random = new();

    private void Awake() => Instance = this;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GameView.Instance.AddLetter(new LetterModel(new StampModel(StampType.MonaLisa, (float)random.NextDouble()), "Recipient\nStreet\nCity\nCountry", "Sender\nStreet\nCity\nCountry"));
    }
}