using System;
using TMPro;
using UnityEngine;

public class LetterView : MonoBehaviour
{
    public enum SortingOrder
    {
        Shadow,
        Envelope,
        Text,
        Stamp
    }

    [SerializeField] private SpriteRenderer envelopeRenderer;
    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private StampView stampView;
    [SerializeField] private TMP_Text recipientText;
    [SerializeField] private Renderer recipientRenderer;
    [SerializeField] private TMP_Text senderText;
    [SerializeField] private Renderer senderRenderer;

    public LetterModel Model { get; private set; }
    public int OrderInLayer { get; private set; }

    public void LinkToModel(LetterModel model)
    {
        Model = model;
        stampView.LinkToModel(model.Stamp);
        UpdateView();
    }

    public void UpdateView()
    {
        if (Model == null)
            return;
        stampView.UpdateView();
        recipientText.text = Model.Recipient;
        senderText.text = Model.Sender;
    }

    public void SetOrderInLayer(int order)
    {
        OrderInLayer = order;
        foreach (var so in Enum.GetValues(typeof(SortingOrder)))
        foreach (var r in GetRenderers((SortingOrder)so))
            r.sortingOrder = order + (int)so;
        stampView.SetOrderInLayer(order + (int)SortingOrder.Stamp);
    }

    private Renderer[] GetRenderers(SortingOrder sortingOrder) => sortingOrder switch
    {
        SortingOrder.Shadow => new Renderer[] { shadowRenderer },
        SortingOrder.Envelope => new Renderer[] { envelopeRenderer },
        SortingOrder.Text => new[] { recipientRenderer, senderRenderer },
        _ => Array.Empty<Renderer>()
    };
}