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
    }

    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private SpriteRenderer envelopeRenderer;
    [SerializeField] private SpriteRenderer holeRenderer;
    [SerializeField] private BoxCollider2D envelopeCollider;
    [SerializeField] private TMP_Text recipientText;
    [SerializeField] private Renderer recipientRenderer;
    [SerializeField] private TMP_Text senderText;
    [SerializeField] private Renderer senderRenderer;
    [SerializeField] private StampView stampView;
    [SerializeField] private Draggable2D draggable;

    public LetterModel Model { get; private set; }
    public int OrderInLayer { get; private set; }
    public bool Dragging => draggable.Dragging;
    public SpriteRenderer HoleRenderer => holeRenderer;

    private Sprite mask;

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
        shadowRenderer.size = Model.EnvelopeSize + Vector2.one;
        envelopeRenderer.size = Model.EnvelopeSize;
        envelopeRenderer.color = Model.EnvelopeColor;
        envelopeCollider.size = Model.EnvelopeSize;
        recipientText.text = Model.Recipient;
        senderText.text = Model.Sender;
        stampView.UpdateView();
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
        stampView.SetOrderInLayer(order + Enum.GetValues(typeof(SortingOrder)).Length);
    }

    private Renderer[] GetRenderers(SortingOrder sortingOrder) => sortingOrder switch
    {
        SortingOrder.Shadow => new Renderer[] { shadowRenderer },
        SortingOrder.Envelope => new Renderer[] { envelopeRenderer },
        SortingOrder.Text => new[] { holeRenderer, recipientRenderer, senderRenderer },
        _ => Array.Empty<Renderer>()
    };
}