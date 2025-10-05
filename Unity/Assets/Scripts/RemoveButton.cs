using UnityEngine;

public class RemoveButton : MonoBehaviour
{
    [SerializeField] private CollectionView collectionView;
    [SerializeField] private int index;

    private Camera cam;
    private bool isHovered;
    private bool mouseOver;

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
        mouseOver = true;
    }

    private void OnMouseExitVisible()
    {
        mouseOver = false;
    }

    public void OnClick()
    {
        if (mouseOver)
            collectionView.SetStamp(null, index);
    }
}