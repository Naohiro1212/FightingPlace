using UnityEngine;

public class PlayerToSafeZone : MonoBehaviour
{
    private PlayerStatus status;

    [SerializeField]
    private SafeZoneManager manager;

    [Header("Zone Damage")]
    [SerializeField]
    private float maxOutTimer = 1.0f;

    [SerializeField]
    private int zoneDamage = 10;

    private float outOfZoneTimer = 0.0f;

    private void Start()
    {
        status = GetComponent<PlayerStatus>();

        manager = FindAnyObjectByType<SafeZoneManager>();

        if (manager == null)
        {
            Debug.LogError("SafeZoneManagerが設定されていません");
        }

        if (status == null)
        {
            Debug.LogError("PlayerStatusがありません");
        }
    }

    private void Update()
    {
        if (manager == null || status == null)
        {
            return;
        }

        // 安置の外にいる
        if (!manager.IsInsideZone(transform.position))
        {
            outOfZoneTimer += Time.deltaTime;

            if (outOfZoneTimer >= maxOutTimer)
            {
                outOfZoneTimer = 0.0f;

                status.TakeDamage(zoneDamage);
            }
        }
        else
        {
            // 安置内に戻ったらタイマーリセット
            outOfZoneTimer = 0.0f;
        }
    }
}