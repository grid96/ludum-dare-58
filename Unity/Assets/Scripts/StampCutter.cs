using System.Collections.Generic;
using UnityEngine;

public class StampCutter : MonoBehaviour
{
    [SerializeField] private LetterView letterView;
    [SerializeField] private StampView stampView;
    [SerializeField] private SpriteRenderer anchorRenderer;
    [SerializeField] private Vector2 spriteSizeUnits = new(1, 1);
    [SerializeField] private float pixelsPerUnit = 100;
    [SerializeField] private float initialAlpha = 1;
    [SerializeField] private int brushDiameterPx = 5;
    [SerializeField] private Vector2 insideRectCenterLocal = Vector2.zero;
    [SerializeField] private Vector2 insideRectSizeLocal = new(1, 1);
    [SerializeField] private Vector2 outsideBoundsCenterLocal = Vector2.zero;
    [SerializeField] private Vector2 outsideBoundsSizeLocal = new(2, 2);
    [SerializeField] private bool drawGizmos;

    private Texture2D tex;
    private Color32[] pixels;
    private int texW, texH;
    private bool dirty;
    private Sprite sprite;
    private bool mouseOver;

    private struct Offset
    {
        public int dx, dy;
    }

    private List<Offset> brush;
    private bool isDrawing;
    private Vector2 lastLocal;
    private Camera cam;

    private float BrushRadiusWorld => brushDiameterPx * 0.5f / Mathf.Max(1, pixelsPerUnit);

    private void Awake()
    {
        if (outsideBoundsSizeLocal.x <= 0 || outsideBoundsSizeLocal.y <= 0)
        {
            outsideBoundsCenterLocal = Vector2.zero;
            outsideBoundsSizeLocal = spriteSizeUnits;
        }

        BuildBrushKernel();
        CreateAndAssignSprite();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!cam || !tex)
        {
            ApplyIfDirty();
            return;
        }

        var world = cam.ScreenToWorldPoint(Input.mousePosition);
        var local = anchorRenderer.transform.InverseTransformPoint(world);
        float r = BrushRadiusWorld;
        var outside = LocalRect(outsideBoundsCenterLocal, outsideBoundsSizeLocal);
        var clampedLocal = new Vector2(Mathf.Clamp(local.x, outside.xMin + r, outside.xMax - r), Mathf.Clamp(local.y, outside.yMin + r, outside.yMax - r));

        if (Input.GetMouseButtonDown(0) && Toolbar.Instance.CurrentTool == Toolbar.Tool.Knife && !Toolbar.Instance.MouseOver && mouseOver)
        {
            isDrawing = true;
            lastLocal = clampedLocal;
            DrawAtLocal(clampedLocal);
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            DrawLineLocal(lastLocal, clampedLocal);
            lastLocal = clampedLocal;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDrawing)
            {
                isDrawing = false;
                ApplyIfDirty();
                var alphaPixels = tex.GetPixels32();
                bool ok = FloodRegionContainsRectNoBorder(alphaPixels, out bool touchedBorder, out var visitedIdx);
                if (ok)
                    GenerateInsideOutsideSpritesFromFlood(alphaPixels, visitedIdx);
                else
                    ResetMask();
            }
        }

        ApplyIfDirty();
    }

#if UNITY_EDITOR
    [ContextMenu("Recreate Sprite")]
    private void Recreate() => CreateAndAssignSprite();
#endif

    private void CreateAndAssignSprite()
    {
        spriteSizeUnits.x = Mathf.Max(0.001f, spriteSizeUnits.x);
        spriteSizeUnits.y = Mathf.Max(0.001f, spriteSizeUnits.y);
        pixelsPerUnit = Mathf.Max(1, pixelsPerUnit);
        texW = Mathf.RoundToInt(spriteSizeUnits.x * pixelsPerUnit);
        texH = Mathf.RoundToInt(spriteSizeUnits.y * pixelsPerUnit);
        tex = new Texture2D(texW, texH, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        pixels = new Color32[texW * texH];
        byte a = (byte)Mathf.RoundToInt(initialAlpha * 255f);
        var fill = new Color32(255, 255, 255, a);
        for (int i = 0; i < pixels.Length; i++) pixels[i] = fill;
        tex.SetPixels32(pixels);
        tex.Apply();
        sprite = Sprite.Create(tex, new Rect(0, 0, texW, texH), new Vector2(0.5f, 0.5f), pixelsPerUnit, 0, SpriteMeshType.FullRect);
        anchorRenderer.sprite = sprite;
        stampView.Mask.sprite = sprite;
        dirty = false;
    }

    private void BuildBrushKernel()
    {
        brush = new List<Offset>();
        int rPix = Mathf.Max(1, Mathf.RoundToInt(brushDiameterPx * 0.5f));
        float radius = rPix + 0.5f;
        for (int dy = -rPix; dy <= rPix; dy++)
        for (int dx = -rPix; dx <= rPix; dx++)
        {
            float d = Mathf.Sqrt(dx * dx + dy * dy);
            if (d <= radius)
                brush.Add(new Offset { dx = dx, dy = dy });
        }
    }

    private void DrawLineLocal(Vector2 aLocal, Vector2 bLocal)
    {
        float distPx = (bLocal - aLocal).magnitude * pixelsPerUnit;
        int steps = Mathf.Max(1, Mathf.CeilToInt(distPx * 2));
        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 1 : (float)i / steps;
            var p = Vector2.Lerp(aLocal, bLocal, t);
            DrawAtLocal(p);
        }
    }

    private void DrawAtLocal(Vector2 local)
    {
        var half = spriteSizeUnits * 0.5f;
        int cx = Mathf.RoundToInt((local.x + half.x) * pixelsPerUnit);
        int cy = Mathf.RoundToInt((local.y + half.y) * pixelsPerUnit);
        EraseCircle(cx, cy);
    }

    private void EraseCircle(int cx, int cy)
    {
        for (int i = 0; i < brush.Count; i++)
        {
            int x = cx + brush[i].dx;
            int y = cy + brush[i].dy;
            if ((uint)x >= (uint)texW || (uint)y >= (uint)texH)
                continue;
            pixels[y * texW + x].a = 0;
        }

        dirty = true;
    }

    private void ApplyIfDirty()
    {
        if (!dirty)
            return;
        tex.SetPixels32(pixels);
        tex.Apply(false, false);
        dirty = false;
    }

    private void ResetMask()
    {
        byte a = (byte)Mathf.RoundToInt(initialAlpha * 255);
        var fill = new Color32(255, 255, 255, a);
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = fill;
        tex.SetPixels32(pixels);
        tex.Apply(false, false);
        if (stampView.Mask)
        {
            var s = stampView.Mask.sprite;
            stampView.Mask.sprite = null;
            stampView.Mask.sprite = s ?? sprite;
        }

        if (anchorRenderer)
        {
            var r = anchorRenderer.sprite;
            anchorRenderer.sprite = null;
            anchorRenderer.sprite = r ?? sprite;
        }

        dirty = false;
    }

    private bool FloodRegionContainsRectNoBorder(Color32[] alphaPixels, out bool touchedBorder, out HashSet<int> visitedIdx)
    {
        touchedBorder = false;
        visitedIdx = new HashSet<int>();

        var half = spriteSizeUnits * 0.5f;

        var outsideLocal = LocalRect(outsideBoundsCenterLocal, outsideBoundsSizeLocal);
        int obMinX = Mathf.RoundToInt((outsideLocal.xMin + half.x) * pixelsPerUnit);
        int obMaxX = Mathf.RoundToInt((outsideLocal.xMax + half.x) * pixelsPerUnit);
        int obMinY = Mathf.RoundToInt((outsideLocal.yMin + half.y) * pixelsPerUnit);
        int obMaxY = Mathf.RoundToInt((outsideLocal.yMax + half.y) * pixelsPerUnit);

        obMinX = Mathf.Clamp(obMinX, 0, texW - 1);
        obMaxX = Mathf.Clamp(obMaxX, 0, texW - 1);
        obMinY = Mathf.Clamp(obMinY, 0, texH - 1);
        obMaxY = Mathf.Clamp(obMaxY, 0, texH - 1);

        var insideLocal = LocalRect(insideRectCenterLocal, insideRectSizeLocal);
        int inMinX = Mathf.RoundToInt((insideLocal.xMin + half.x) * pixelsPerUnit);
        int inMaxX = Mathf.RoundToInt((insideLocal.xMax + half.x) * pixelsPerUnit);
        int inMinY = Mathf.RoundToInt((insideLocal.yMin + half.y) * pixelsPerUnit);
        int inMaxY = Mathf.RoundToInt((insideLocal.yMax + half.y) * pixelsPerUnit);

        int startX = Mathf.RoundToInt(((insideRectCenterLocal.x) + half.x) * pixelsPerUnit);
        int startY = Mathf.RoundToInt(((insideRectCenterLocal.y) + half.y) * pixelsPerUnit);

        if (!IsInsideTex(startX, startY) || alphaPixels[startY * texW + startX].a == 0)
            return false;

        Queue<(int x, int y)> q = new();
        int startIdx = startY * texW + startX;
        visitedIdx.Add(startIdx);
        q.Enqueue((startX, startY));

        while (q.Count > 0)
        {
            var (x, y) = q.Dequeue();
            if (x <= obMinX || x >= obMaxX || y <= obMinY || y >= obMaxY)
            {
                touchedBorder = true;
                break;
            }

            foreach (var (dx, dy) in Neigh4())
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx < obMinX || nx > obMaxX || ny < obMinY || ny > obMaxY)
                {
                    touchedBorder = true;
                    break;
                }

                if (!IsInsideTex(nx, ny))
                {
                    touchedBorder = true;
                    break;
                }

                int nidx = ny * texW + nx;
                if (visitedIdx.Contains(nidx))
                    continue;
                if (alphaPixels[nidx].a == 0)
                    continue;
                visitedIdx.Add(nidx);
                q.Enqueue((nx, ny));
            }

            if (touchedBorder)
                break;
        }

        if (touchedBorder)
            return false;

        for (int y = inMinY; y <= inMaxY; y++)
        for (int x = inMinX; x <= inMaxX; x++)
        {
            if (!IsInsideTex(x, y))
                return false;
            int idx = y * texW + x;
            if (alphaPixels[idx].a == 0)
                return false;
            if (!visitedIdx.Contains(idx))
                return false;
        }

        return true;

        static IEnumerable<(int dx, int dy)> Neigh4()
        {
            yield return (1, 0);
            yield return (-1, 0);
            yield return (0, 1);
            yield return (0, -1);
        }
    }

    private bool IsInsideTex(int x, int y) => (uint)x < (uint)texW && (uint)y < (uint)texH;

    private Rect LocalRect(Vector2 center, Vector2 size)
    {
        var half = size * 0.5f;
        return new Rect(center - half, size);
    }

    private void GenerateInsideOutsideSpritesFromFlood(Color32[] basePixels, HashSet<int> floodVisited)
    {
        var insideClear = new Color32[basePixels.Length];
        var outsideClear = new Color32[basePixels.Length];
        for (int i = 0; i < basePixels.Length; i++)
        {
            bool inFlood = floodVisited.Contains(i);
            var c = basePixels[i];
            insideClear[i] = inFlood ? new Color32(c.r, c.g, c.b, 0) : c;
            outsideClear[i] = inFlood ? c : new Color32(c.r, c.g, c.b, 0);
        }

        var insideSprite = Sprite.Create(CreateTextureFrom(insideClear), new Rect(0, 0, texW, texH), new Vector2(0.5f, 0.5f), pixelsPerUnit, 0, SpriteMeshType.FullRect);
        var outsideSprite = Sprite.Create(CreateTextureFrom(outsideClear), new Rect(0, 0, texW, texH), new Vector2(0.5f, 0.5f), pixelsPerUnit, 0, SpriteMeshType.FullRect);

        var stampClone = Instantiate(stampView);
        stampClone.transform.position = stampView.transform.position;
        stampClone.transform.rotation = stampView.transform.rotation;
        stampClone.LinkToModel(stampView.Model);
        GameView.Instance.AddStamp(stampClone);

        letterView.HoleRenderer.color = Color.black * 0.5f;
        stampView.Mask.sprite = insideSprite;
        stampView.DateText.gameObject.SetActive(false);
        stampClone.Mask.sprite = outsideSprite;
        stampClone.Simulated = true;
        
        Toolbar.Instance.SetTool(Toolbar.Tool.Hand);
            
        Destroy(this);
    }

    private Texture2D CreateTextureFrom(Color32[] src)
    {
        Texture2D t = new Texture2D(texW, texH, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        t.SetPixels32(src);
        t.Apply();
        return t;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || !anchorRenderer)
            return;
        Gizmos.color = Color.yellow;
        DrawRectGizmo(LocalRect(Vector2.zero, spriteSizeUnits));
        Gizmos.color = Color.magenta;
        DrawRectGizmo(LocalRect(outsideBoundsCenterLocal, outsideBoundsSizeLocal));
        Gizmos.color = Color.cyan;
        DrawRectGizmo(LocalRect(insideRectCenterLocal, insideRectSizeLocal));
    }

    private void DrawRectGizmo(Rect rLocal)
    {
        Vector3 a = anchorRenderer.transform.TransformPoint(new Vector3(rLocal.xMin, rLocal.yMin));
        Vector3 b = anchorRenderer.transform.TransformPoint(new Vector3(rLocal.xMin, rLocal.yMax));
        Vector3 c = anchorRenderer.transform.TransformPoint(new Vector3(rLocal.xMax, rLocal.yMax));
        Vector3 d = anchorRenderer.transform.TransformPoint(new Vector3(rLocal.xMax, rLocal.yMin));
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
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