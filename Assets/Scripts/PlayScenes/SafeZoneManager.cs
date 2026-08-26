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

    private void Start()
    {
        currentCenter = transform.position;
        currentRadius = startRadius;

        zoneLine.useWorldSpace = true;

        StartCoroutine(ZoneRoutine());
    }

    private void Update()
    {
        DrawCircle();
    }

    private IEnumerator ZoneRoutine()
    {
        while (currentRadius > 3.0f)
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
        nextRadius = currentRadius * nextRadiusRate;

        // 次の円が現在の円からはみ出さない最大距離
        float maxOffset = currentRadius - nextRadius;

        float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
        float distance = Random.Range(0.0f, maxOffset);

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

    // ゾーンを縮小する
    private IEnumerator ShrinkZone()
    {
        Vector3 startCenter = currentCenter;
        float startRadiusValue = currentRadius;

        float timer = 0.0f;

        while (timer < shrinkTime)
        {
            timer += Time.deltaTime;

            float t = timer / shrinkTime;

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

        currentCenter = nextCenter;
        currentRadius = nextRadius;
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
