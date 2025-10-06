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
    public int OrderStep => Enum.GetValues(typeof(LetterView.SortingOrder)).Length + Enum.GetValues(typeof(StampView.SortingOrder)).Length + 3;
    public int FolderOrderStep => Enum.GetValues(typeof(FolderView.SortingOrder)).Length + Enum.GetValues(typeof(CollectionView.SortingOrder)).Length + Enum.GetValues(typeof(StampView.SortingOrder)).Length + 3;

    private readonly List<LetterView> letterViews = new();
    private readonly List<StampView> stampViews = new();
    private Random random = new((uint)DateTime.Now.Millisecond);

    private void Awake() => Instance = this;

    private void Start()
    {
        LinkToModel(new GameModel());
        FolderView.Instance.LinkToModel(Model.Folder);
        Postman.Instance.AddLetter(false);
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

        while (letterViews.Count + stampViews.Count > 50)
        {
            var letterView = letterViews.OrderBy(v => v.OrderInLayer).FirstOrDefault();
            var stampView = stampViews.OrderBy(v => v.OrderInLayer).FirstOrDefault();
            if (letterView != null && (stampView == null || letterView.OrderInLayer < stampView.OrderInLayer))
            {
                Model.Letters.Remove(letterView.Model);
                letterViews.Remove(letterView);
                Destroy(letterView.gameObject);
            }
            else if (stampView != null)
            {
                Model.Stamps.Remove(stampView.Model);
                stampViews.Remove(stampView);
                Destroy(stampView.gameObject);
            }
        }

        if (NextOrder > 11000)
        {
            FolderView.Instance.SetOrderInLayer(FolderView.Instance.OrderInLayer - 10000);
            foreach (var view in letterViews)
                view.SetOrderInLayer(view.OrderInLayer - 10000);
            foreach (var view in stampViews)
                view.SetOrderInLayer(view.OrderInLayer - 10000);
            NextOrder -= 10000;
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

    public void RemoveStamp(StampView stamp)
    {
        Model.Stamps.Remove(stamp.Model);
        stampViews.Remove(stamp);
    }
}