using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Draggable2D : MonoBehaviour
{
    private const int VelSamples = 5;
    
    [SerializeField] private float followSmoothing = 0.5f;
    [SerializeField] private float impulseScale = 1;
    [SerializeField] private float suggestedLinearDampingIfZero = 5;
    [SerializeField] private LetterView letterView;
    [SerializeField] private StampView stampView;
    
    public bool Dragging => dragging;

    private Camera cam;
    private Rigidbody2D rb;
    private bool dragging;
    private Vector2 localGrabOffset;
    private readonly Vector2[] pointerBodyPos = new Vector2[VelSamples];
    private readonly float[] pointerDt = new float[VelSamples];
    private int sampleIdx;
    private float savedGravity;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        if (Mathf.Approximately(rb.linearDamping, 0) && suggestedLinearDampingIfZero > 0)
            rb.linearDamping = suggestedLinearDampingIfZero;
    }

    private void OnMouseDown()
    {
        if (Toolbar.Instance.CurrentTool != Toolbar.Tool.Hand || Toolbar.Instance.MouseOver)
            return;
        if (!cam)
            return;
        dragging = true;
        sampleIdx = 0;
        for (int i = 0; i < VelSamples; i++)
            pointerDt[i] = 0;
        Vector3 w = cam.ScreenToWorldPoint(Input.mousePosition);
        w.z = 0;
        localGrabOffset = transform.InverseTransformPoint(w);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        savedGravity = rb.gravityScale;
        rb.gravityScale = 0;
        Vector2 bodyTarget = ComputeBodyTargetFromPointer();
        pointerBodyPos[0] = bodyTarget;
        if (letterView)
            letterView.SetOrderInLayer(GameView.Instance.NextOrder += GameView.Instance.OrderStep);
        if (stampView)
            stampView.SetOrderInLayer(GameView.Instance.NextOrder += GameView.Instance.OrderStep);
    }

    private void OnMouseDrag()
    {
        if (!dragging || !cam)
            return;
        var bodyTarget = ComputeBodyTargetFromPointer();
        var newPos = followSmoothing <= 0 ? bodyTarget : Vector2.Lerp(rb.position, bodyTarget, 1 - Mathf.Pow(1 - followSmoothing, Time.deltaTime * 60));
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        sampleIdx = (sampleIdx + 1) % VelSamples;
        pointerBodyPos[sampleIdx] = newPos;
        pointerDt[sampleIdx] = dt;
        rb.MovePosition(newPos);
    }

    private void OnMouseUp()
    {
        if (!dragging)
            return;
        dragging = false;
        rb.gravityScale = savedGravity;
        rb.constraints = RigidbodyConstraints2D.None;
        var v = EstimatePointerVelocity();
        var impulse = v * rb.mass * impulseScale;
        if (impulse.sqrMagnitude > 0)
            rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    private Vector2 ComputeBodyTargetFromPointer()
    {
        var cursor = cam.ScreenToWorldPoint(Input.mousePosition);
        cursor.z = 0;
        var grabRelWorld = transform.TransformVector(localGrabOffset);
        return cursor - grabRelWorld;
    }

    private Vector2 EstimatePointerVelocity()
    {
        var sum = Vector2.zero;
        float totalDt = 0;
        for (int i = 0; i < VelSamples - 1; i++)
        {
            int a = (sampleIdx - i + VelSamples) % VelSamples;
            int b = (a - 1 + VelSamples) % VelSamples;
            float dt = pointerDt[a];
            if (dt <= 0)
                continue;
            sum += pointerBodyPos[a] - pointerBodyPos[b];
            totalDt += dt;
        }

        return totalDt > 0 ? sum / totalDt : Vector2.zero;
    }
}