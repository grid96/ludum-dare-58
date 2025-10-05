using System;
using TMPro;
using UnityEngine;
using Tool = Toolbar.Tool;

public class StampView : MonoBehaviour
{
    public enum SortingOrder
    {
        Background,
        Shadow,
        Cut,
        Glow,
        Stamp,
        Art,
        Damage,
        Postmark,
    }

    [SerializeField] private SpriteMask cutMask;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private SpriteRenderer cutRenderer;
    [SerializeField] private SpriteRenderer glowRenderer;
    [SerializeField] private Color[] rarityColors;
    [SerializeField] private SpriteRenderer stampRenderer;
    [SerializeField] private SpriteRenderer artRenderer;
    [SerializeField] private ArtLibrary artLibrary;
    [SerializeField] private SpriteRenderer damageRenderer;
    [SerializeField] private SpriteRenderer postmarkRenderer;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private Renderer dateRenderer;
    [SerializeField] private Draggable2D draggable;
    [SerializeField] private Rigidbody2D body;

    public StampModel Model { get; private set; }
    public int OrderInLayer { get; private set; }

    public bool Simulated
    {
        get => body.simulated;
        set => body.simulated = value;
    }

    public bool Dragging => draggable.Dragging;
    public SpriteMask Mask => cutMask;
    public TMP_Text DateText => dateText;

    private Sprite mask;
    private bool mouseOver;

    public void LinkToModel(StampModel model)
    {
        Model = model;
        UpdateView();
    }

    public void UpdateView()
    {
        if (Model == null)
            return;
        backgroundRenderer.color = Model.BackgroundColor;
        glowRenderer.color = rarityColors[(int)Model.Rarity];
        artRenderer.sprite = artLibrary.Get(Model.Type);
        var damageColor = damageRenderer.color;
        damageColor.a = Model.Damage;
        damageRenderer.color = damageColor;
        postmarkRenderer.transform.localPosition = Model.PostmarkOffset;
        postmarkRenderer.transform.localRotation = Quaternion.Euler(0, 0, Model.PostmarkRotation);
        var postmarkColor = Model.PostmarkColor;
        postmarkColor.a = Model.Postmark;
        postmarkRenderer.color = postmarkColor;
        dateText.text = Model.PostmarkDate.ToString("dd.MM.yyyy");
        dateText.color = postmarkColor;
    }

    public void SetOrderInLayer(int order)
    {
        OrderInLayer = order;
        var position = transform.position;
        position.z = order * -0.001f;
        transform.position = position;
        cutMask.frontSortingOrder = order + Enum.GetValues(typeof(SortingOrder)).Length;
        cutMask.backSortingOrder = order - 1;
        foreach (var so in Enum.GetValues(typeof(SortingOrder)))
        foreach (var r in GetRenderers((SortingOrder)so))
            r.sortingOrder = order + (int)so;
    }

    public void SetMask(Sprite sprite)
    {
        cutMask.sprite = sprite;
    }

    private Renderer[] GetRenderers(SortingOrder sortingOrder) => sortingOrder switch
    {
        SortingOrder.Background => new Renderer[] { backgroundRenderer },
        SortingOrder.Shadow => new Renderer[] { shadowRenderer },
        SortingOrder.Cut => new Renderer[] { cutRenderer },
        SortingOrder.Glow => new Renderer[] { glowRenderer },
        SortingOrder.Stamp => new Renderer[] { stampRenderer },
        SortingOrder.Art => new Renderer[] { artRenderer },
        SortingOrder.Damage => new Renderer[] { damageRenderer },
        SortingOrder.Postmark => new[] { postmarkRenderer, dateRenderer },
        _ => Array.Empty<Renderer>()
    };

    private void Update()
    {
        foreach (var r in GetRenderers(SortingOrder.Cut))
            r.gameObject.SetActive(Toolbar.Instance.CurrentTool == Tool.Knife && !Simulated);
        if (!Input.GetMouseButton(0) || !mouseOver || Toolbar.Instance.CurrentTool != Tool.Sponge || Toolbar.Instance.MouseOver)
            return;
        if (Model.Postmark > 0)
            Model.Postmark = Mathf.Clamp01(Model.Postmark - Input.mousePositionDelta.magnitude * 0.001f);
        else
            Model.Damage = Mathf.Clamp01(Model.Damage + Input.mousePositionDelta.magnitude * 0.002f);
        UpdateView();
    }

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }
}