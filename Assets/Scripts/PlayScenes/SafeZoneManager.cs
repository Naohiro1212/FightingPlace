using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SafeZoneManager : NetworkBehaviour
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

    private NetworkVariable<Vector3> currentCenter =
        new NetworkVariable<Vector3>(
            Vector3.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private NetworkVariable<float> currentRadius =
        new NetworkVariable<float>(
            0.0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private Vector3 nextCenter;
    private float nextRadius;

    [SerializeField]
    private MeshFilter dangerZoneMeshFilter;

    [SerializeField]
    private float dangerOuterRadius = 150.0f;

    private Mesh dangerZoneMesh;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        zoneLine.useWorldSpace = true;

        dangerZoneMesh = new Mesh();
        dangerZoneMeshFilter.mesh = dangerZoneMesh;

        // 安置の計算はServerだけ
        if (IsServer)
        {
            currentCenter.Value = transform.position;
            currentRadius.Value = startRadius;

            StartCoroutine(ZoneRoutine());
        }
    }

    private void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        // NetworkVariableで同期された中心に合わせる
        transform.position = currentCenter.Value;

        DrawCircle();
        DrawDangerZone();
    }

    private IEnumerator ZoneRoutine()
    {
        while (currentRadius.Value > 0.0f)
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
        if (!IsServer)
        {
            return;
        }

        float rate =
            Mathf.Clamp01(nextRadiusRate);

        nextRadius =
            currentRadius.Value * rate;

        if (nextRadius < 1.0f)
        {
            nextRadius = 0.0f;
        }

        float maxOffset =
            currentRadius.Value - nextRadius;

        float angle =
            Random.Range(
                0.0f,
                Mathf.PI * 2.0f
            );

        float distance =
            Random.Range(
                0.0f,
                maxOffset
            );

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            0.0f,
            Mathf.Sin(angle) * distance
        );

        nextCenter =
            currentCenter.Value + offset;
    }

    private void DrawCircle()
    {
        zoneLine.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2.0f;

            Vector3 position = new Vector3(
                Mathf.Cos(angle) * currentRadius.Value,
                0.1f,
                Mathf.Sin(angle) * currentRadius.Value);

            zoneLine.SetPosition(
                i,
                currentCenter.Value + position);
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
                cos * currentRadius.Value,
                1.0f,
                sin * currentRadius.Value
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
        if (!IsServer)
        {
            yield break;
        }

        Vector3 startCenter =
            currentCenter.Value;

        float startRadiusValue =
            currentRadius.Value;

        float timer = 0.0f;

        while (timer < shrinkTime)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / shrinkTime
                );

            currentCenter.Value =
                Vector3.Lerp(
                    startCenter,
                    nextCenter,
                    t
                );

            currentRadius.Value =
                Mathf.Lerp(
                    startRadiusValue,
                    nextRadius,
                    t
                );

            yield return null;
        }

        currentCenter.Value = nextCenter;
        currentRadius.Value = nextRadius;
    }

    public bool IsInsideZone(Vector3 position)
    {
        Vector2 playerPos = new Vector2(
            position.x,
            position.z);

        Vector2 ZonePos = new Vector2(
            currentCenter.Value.x,
            currentCenter.Value.z);

        return Vector2.Distance(
            playerPos,
            ZonePos) <= currentRadius.Value;
    }

    public float getRadius()
    {
        return currentRadius.Value;
    }

    public Vector3 GetCenter()
    {
        return currentCenter.Value;
    }
}
