using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GameView : MonoBehaviour
{
    public static GameView Instance { get; private set; }

    [SerializeField] private Transform letterContainer;
    [SerializeField] private LetterView letterPrefab;

    public GameModel Model { get; private set; }

    private readonly List<LetterView> letterViews = new();
    private Random random = new((uint)System.DateTime.Now.Millisecond);

    private void Awake() => Instance = this;

    private void Start() => LinkToModel(new GameModel());

    public void LinkToModel(GameModel model)
    {
        Model = model;
        foreach (var letter in model.Letters)
            AddLetterView(letter);
        UpdateView();
    }

    public void UpdateView()
    {
        foreach (var view in letterViews)
            view.UpdateView();
    }

    private void AddLetterView(LetterModel letter)
    {
        var view = Instantiate(letterPrefab, letterContainer);
        view.LinkToModel(letter);
        view.SetOrderInLayer(letterViews.Count * 10);
        view.transform.localPosition = new Vector3(random.NextFloat(-5, 5), random.NextFloat(-2, 2), 0);
        view.transform.localRotation = Quaternion.Euler(0, 0, random.NextFloat(-15, 15));
        letterViews.Add(view);
    }

    public void AddLetter(LetterModel letter)
    {
        Model.Letters.Add(letter);
        AddLetterView(letter);
    }
}