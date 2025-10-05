using System;
using TMPro;
using UnityEngine;

public class FolderView : MonoBehaviour
{
    public static FolderView Instance { get; private set; }

    public enum SortingOrder
    {
        Shadow,
        Border,
        Background,
    }

    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private CollectionView[] collectionViews;
    [SerializeField] private TMP_Text scoreText;

    public FolderModel Model { get; private set; }
    public int OrderInLayer { get; private set; }

    private void Awake() => Instance = this;

    public void LinkToModel(FolderModel model)
    {
        Model = model;
        for (int i = 0; i < collectionViews.Length; i++)
            collectionViews[i].LinkToModel(model.Collections[i]);
        UpdateView();
    }

    public void UpdateView()
    {
        foreach (var view in collectionViews)
            view.UpdateView();
        scoreText.text = $"Score: {Model.TotalScore}";
    }

    public void SetOrderInLayer(int order)
    {
        OrderInLayer = order;
        var position = transform.position;
        position.z = order * -0.001f;
        transform.position = position;
        foreach (var so in Enum.GetValues(typeof(SortingOrder)))
        foreach (var r in GetRenderers((SortingOrder)so))
            r.sortingOrder = order + (int)so;
        foreach (var view in collectionViews)
            view.SetOrderInLayer(order + Enum.GetValues(typeof(SortingOrder)).Length);
    }

    private Renderer[] GetRenderers(SortingOrder sortingOrder) => sortingOrder switch
    {
        SortingOrder.Shadow => new Renderer[] { shadowRenderer },
        SortingOrder.Border => new Renderer[] { borderRenderer },
        SortingOrder.Background => new Renderer[] { backgroundRenderer },
        _ => Array.Empty<Renderer>()
    };

    public void OnFolderClick()
    {
        if (transform.localPosition.x < -15 || transform.localPosition.x > 15 || transform.localPosition.y < -10 || transform.localPosition.y > 10)
            transform.localPosition = new Vector3(1.5f, -8, 0);
        SetOrderInLayer(GameView.Instance.NextOrder += GameView.Instance.FolderOrderStep);
    }
}