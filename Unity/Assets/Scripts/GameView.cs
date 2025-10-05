using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GameView : MonoBehaviour
{
    public static GameView Instance { get; private set; }

    [SerializeField] private Transform letterContainer;
    [SerializeField] private LetterView letterPrefab;

    public GameModel Model { get; private set; }
    public int NextOrder { get; set; }
    public int OrderStep => Enum.GetValues(typeof(LetterView.SortingOrder)).Length + Enum.GetValues(typeof(StampView.SortingOrder)).Length;

    private readonly List<LetterView> letterViews = new();
    private readonly List<StampView> stampViews = new();
    private Random random = new((uint)DateTime.Now.Millisecond);

    private void Awake() => Instance = this;

    private void Start()
    {
        LinkToModel(new GameModel());
        Postman.Instance.AddLetter();
    }

    private void Update()
    {
        foreach (var view in letterViews.ToList())
            if (!view.Dragging && (Math.Abs(view.transform.localPosition.x) > 9 + view.Model.EnvelopeSize.magnitude / 2 || Math.Abs(view.transform.localPosition.y) > 5 + view.Model.EnvelopeSize.magnitude / 2))
            {
                Model.Letters.Remove(view.Model);
                letterViews.Remove(view);
                Destroy(view.gameObject);
            }

        foreach (var view in stampViews.ToList())
            if (!view.Dragging && (Math.Abs(view.transform.localPosition.x) > 9 + new Vector2(2, 2.5f).magnitude / 2 || Math.Abs(view.transform.localPosition.y) > 5 + new Vector2(2, 2.5f).magnitude / 2))
            {
                Model.Stamps.Remove(view.Model);
                stampViews.Remove(view);
                Destroy(view.gameObject);
            }
    }

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
        foreach (var view in stampViews)
            view.UpdateView();
    }

    private void AddLetterView(LetterModel letter)
    {
        var view = Instantiate(letterPrefab, letterContainer);
        view.LinkToModel(letter);
        view.SetOrderInLayer(NextOrder += OrderStep);
        view.transform.localPosition = new Vector3(random.NextFloat(-5, 5), random.NextFloat(-2, 2), view.transform.localPosition.z);
        view.transform.localRotation = Quaternion.Euler(0, 0, random.NextFloat(-15, 15));
        letterViews.Add(view);
    }

    public void AddLetter(LetterModel letter)
    {
        Model.Letters.Add(letter);
        AddLetterView(letter);
    }

    public void AddStamp(StampView stamp)
    {
        Model.Stamps.Add(stamp.Model);
        stamp.transform.SetParent(letterContainer);
        stamp.SetOrderInLayer(NextOrder += OrderStep);
        stampViews.Add(stamp);
    }
}