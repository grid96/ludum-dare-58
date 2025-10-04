using System;
using UnityEngine;

public class StampView : MonoBehaviour
{
    public enum SortingOrder
    {
        Shadow,
        Glow,
        Stamp,
        Art
    }
    
    [SerializeField] private SpriteRenderer stampRenderer;
    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private SpriteRenderer glowRenderer;
    [SerializeField] private SpriteRenderer artRenderer;
    
    public StampModel Model { get; private set; }
    public int OrderInLayer { get; private set; }
    
    public void LinkToModel(StampModel model)
    {
        Model = model;
        UpdateView();
    }
    
    public void UpdateView()
    {
        if (Model == null)
            return;
        artRenderer.sprite = ArtLibrary.Instance.Get(Model.Type);
    }

    public void SetOrderInLayer(int order)
    {
        OrderInLayer = order;
        foreach (var so in Enum.GetValues(typeof(SortingOrder)))
        foreach (var r in GetRenderers((SortingOrder)so))
            r.sortingOrder = order + (int)so;
    }
    
    private Renderer[] GetRenderers(SortingOrder sortingOrder) => sortingOrder switch
    {
        SortingOrder.Shadow => new Renderer[] { shadowRenderer },
        SortingOrder.Glow => new Renderer[] { glowRenderer },
        SortingOrder.Stamp => new Renderer[] { stampRenderer },
        SortingOrder.Art => new Renderer[] { artRenderer },
        _ => Array.Empty<Renderer>()
    };
}