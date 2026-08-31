using System.Collections;
using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    [Header("SafeZone")]
    [SerializeField] 
    private float startRadius;

    [SerializeField]
    private float nextRadiusRate = 0.7f;

    [SerializeField]
    private LineRenderer zoneLine;

    [SerializeField]
    private int segments = 100;

    [Header("Time")]
    [SerializeField]
    private float waitTime = 10.0f;

    [SerializeField]
    private float shrinkTime = 8.0f;

    private Vector3 currentCenter;
    private float currentRadius;

    private Vector3 nextCenter;
    private float nextRadius;

    [SerializeField]
    private MeshFilter dangerZoneMeshFilter;

    [SerializeField]
    private float dangerOuterRadius = 150.0f;

    private Mesh dangerZoneMesh;

    private void Start()
    {
        currentCenter = transform.position;
        currentRadius = startRadius;

        zoneLine.useWorldSpace = true;

        dangerZoneMesh = new Mesh();
        dangerZoneMeshFilter.mesh = dangerZoneMesh;

        StartCoroutine(ZoneRoutine());
    }

    private void Update()
    {
        DrawCircle();
        DrawDangerZone();
    }

    private IEnumerator ZoneRoutine()
    {
        while (currentRadius > 0.0f)
        {
            // 次の安置を決める
            CreateNextZone();

            // 次の安置を表示して待つ
            yield return new WaitForSeconds(waitTime);

            // 安置縮小
            yield return StartCoroutine(ShrinkZone());
        }
    }

    private void CreateNextZone()
    {
        float rate = Mathf.Clamp01(nextRadiusRate);

        nextRadius = currentRadius * rate;

        // ある程度小さくなったら最後は0
        if (nextRadius < 1.0f)
        {
            nextRadius = 0.0f;
        }

        float maxOffset = currentRadius - nextRadius;

        float angle = Random.Range(
            0.0f,
            Mathf.PI * 2.0f
        );

        float distance = Random.Range(
            0.0f,
            maxOffset
        );

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            0.0f,
            Mathf.Sin(angle) * distance
        );

        nextCenter = currentCenter + offset;
    }

    private void DrawCircle()
    {
        zoneLine.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2.0f;

            Vector3 position = new Vector3(
                Mathf.Cos(angle) * currentRadius,
                0.1f,
                Mathf.Sin(angle) * currentRadius);

            zoneLine.SetPosition(
                i,
                currentCenter + position);
        }
    }

    private void DrawDangerZone()
    {
        dangerZoneMesh.Clear();

        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float angle =
                ((float)i / segments) *
                Mathf.PI * 2.0f;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // 安置の境界
            vertices[i * 2] = new Vector3(
                cos * currentRadius,
                1.0f,
                sin * currentRadius
            );

            // 赤エリアの外側
            vertices[i * 2 + 1] = new Vector3(
                cos * dangerOuterRadius,
                1.0f,
                sin * dangerOuterRadius
            );
        }

        for (int i = 0; i < segments; i++)
        {
            int vertexIndex = i * 2;
            int triangleIndex = i * 6;

            triangles[triangleIndex] =
                vertexIndex;

            triangles[triangleIndex + 1] =
                vertexIndex + 1;

            triangles[triangleIndex + 2] =
                vertexIndex + 2;

            triangles[triangleIndex + 3] =
                vertexIndex + 2;

            triangles[triangleIndex + 4] =
                vertexIndex + 1;

            triangles[triangleIndex + 5] =
                vertexIndex + 3;
        }

        dangerZoneMesh.vertices = vertices;
        dangerZoneMesh.triangles = triangles;

        dangerZoneMesh.RecalculateBounds();
    }

    // ゾーンを縮小する
    private IEnumerator ShrinkZone()
    {
        Vector3 startCenter = currentCenter;
        float startRadiusValue = currentRadius;

        float timer = 0.0f;

        while (timer < shrinkTime)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / shrinkTime
            );

            currentCenter = Vector3.Lerp(
                startCenter,
                nextCenter,
                t
            );

            currentRadius = Mathf.Lerp(
                startRadiusValue,
                nextRadius,
                t
            );

            transform.position = currentCenter;

            yield return null;
        }

        // 最後の値を確実に一致させる
        currentCenter = nextCenter;
        currentRadius = nextRadius;

        transform.position = currentCenter;
    }

    public bool IsInsideZone(Vector3 position)
    {
        Vector2 playerPos = new Vector2(
            position.x,
            position.z);

        Vector2 ZonePos = new Vector2(
            currentCenter.x,
            currentCenter.z);

        return Vector2.Distance(
            playerPos,
            ZonePos) <= currentRadius;
    }

    public float getRadius()
    {
        return currentRadius;
    }

    public Vector3 GetCenter()
    {
        return currentCenter;
    }
}
