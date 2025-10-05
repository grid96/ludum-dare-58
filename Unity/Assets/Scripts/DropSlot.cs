using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour
{
    public static DropSlot MouseOver { get; private set; }

    [SerializeField] private CollectionView collectionView;
    [SerializeField] private int index;

    public CollectionModel.Collection Collection => collectionView.Model.Type;
    public CollectionView CollectionView => collectionView;
    public int Index => index;
    private Camera cam;
    private bool isHovered;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);
        Collider2D topCollider = null;
        foreach (var hit in hits)
        {
            if (Draggable2D.Current != null && hit.collider == Draggable2D.Current.Collider)
                continue;
            topCollider = hit.collider;
            break;
        }

        bool hoveredNow = topCollider == GetComponent<Collider2D>();
        if (hoveredNow && !isHovered)
        {
            isHovered = true;
            OnMouseEnterVisible();
        }
        else if (!hoveredNow && isHovered)
        {
            isHovered = false;
            OnMouseExitVisible();
        }
    }

    private void OnMouseEnterVisible()
    {
        MouseOver = this;
        // Debug.Log($"Mouse entered {Collection} slot {Index}");
    }

    private void OnMouseExitVisible()
    {
        if (MouseOver != this)
            return;
        MouseOver = null;
        // Debug.Log($"Mouse exited {Collection} slot {Index}");
    }
}