using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeCircle : MonoBehaviour
{
    public int segments = 50;
    public float radius = 5f;
    public float lineWidth = 0.1f;
    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        DrawCircle();
    }

    // This runs in the Editor whenever you change a value in the Inspector
    void OnValidate()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        DrawCircle();
    }

    public void UpdateRadius(float newRadius)
    {
        radius = newRadius;
        DrawCircle();
    }

    public void DrawCircle()
    {
        if (line == null) return;

        line.positionCount = segments + 1;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.useWorldSpace = false;

        float angle = 0f;
        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / segments);
        }
    }
}