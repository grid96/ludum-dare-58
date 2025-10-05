using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class CollectionView : MonoBehaviour
{
    public enum SortingOrder
    {
        Background,
        Slot,
        Text,
    }

    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private SpriteRenderer[] slotRenderers;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Renderer titleRenderer;
    [SerializeField] private TMP_Text[] scoreTexts;
    [SerializeField] private Color[] scoreColors;
    [SerializeField] private Renderer[] scoreRenderers;
    [SerializeField] private SpriteRenderer[] removeButtons;

    public CollectionModel Model { get; private set; }
    public int OrderInLayer { get; private set; }

    private StampView[] stampViews = new StampView[4];

    public void LinkToModel(CollectionModel model)
    {
        Model = model;
        UpdateView();
    }

    public void UpdateView()
    {
        if (Model == null)
            return;
        Model.CalculateScores();
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            scoreTexts[i].text = Model.Stamps[i] == null ? "" : Model.Scores[i].ToString();
            scoreTexts[i].color = scoreColors[Model.Scores[i]];
        }

        for (int i = 0; i < removeButtons.Length; i++)
            removeButtons[i].gameObject.SetActive(Model.Stamps[i] != null);
    }

    public void SetStamp(StampView view, int index)
    {
        if (stampViews[index] != null && view != null)
            return;
        if (stampViews[index] != null && view == null)
        {
            Model.Stamps[index] = null;
            stampViews[index].transform.localScale = Vector3.one;
            stampViews[index].Simulated = true;
            GameView.Instance.AddStamp(stampViews[index]);
            stampViews[index] = null;
            FolderView.Instance.UpdateView();
            return;
        }

        if (stampViews[index] == null && view != null)
        {
            Model.Stamps[index] = view.Model;
            GameView.Instance.RemoveStamp(view);
            stampViews[index] = view;
            view.Simulated = false;
            view.transform.SetParent(slotRenderers[index].transform);
            view.transform.localScale = Vector3.one * 0.8f;
            view.transform.localPosition = new Vector3(0, 0.35f, 0);
            view.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //view.SetOrderInLayer(GameView.Instance.NextOrder += GameView.Instance.OrderStep);
            FolderView.Instance.SetOrderInLayer(GameView.Instance.NextOrder += GameView.Instance.FolderOrderStep);
            FolderView.Instance.UpdateView();
            _ = DialogManager.Instance.ShowStampInfo(view.Model);
        }
    }

    public void SetOrderInLayer(int order)
    {
        OrderInLayer = order;
        var position = transform.position;
        position.z = order * -0.001f;
        transform.position = position;
        foreach (var so in Enum.GetValues(typeof(SortingOrder)))
        foreach (var r in GetRenderers((SortingOrder)so))
        {
            r.sortingOrder = order + (int)so;
            var position2 = r.transform.position;
            position2.z = (order + (int)so) * -0.001f;
            r.transform.position = position2;
        }

        foreach (var view in stampViews)
            if (view != null)
                view.SetOrderInLayer(order + Enum.GetValues(typeof(SortingOrder)).Length);
    }

    private Renderer[] GetRenderers(SortingOrder sortingOrder) => sortingOrder switch
    {
        SortingOrder.Background => new Renderer[] { backgroundRenderer },
        SortingOrder.Slot => slotRenderers.Cast<Renderer>().ToArray(),
        SortingOrder.Text => new[] { titleRenderer }.Concat(scoreRenderers).Concat(removeButtons).ToArray(),
        _ => Array.Empty<Renderer>()
    };
}