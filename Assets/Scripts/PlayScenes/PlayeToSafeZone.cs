using UnityEngine;

public class PlayerToSafeZone : MonoBehaviour
{
    PlayerStatus status;
    SafeZoneManager manager;
    [SerializeField] private new Transform transform;

    // ゾーン外に出ている際の時間
    private float outOfZoneTimer = 0.0f;
    private float maxOutTimer = 1.0f;
    private int zoneDamage = 10;

    private void Start()
    {
        manager = GetComponent<SafeZoneManager>();
        status = GetComponent<PlayerStatus>();

        if (manager == null)
        {
            Debug.Log("ゾーンマネージャーがありません");
        }

        if (status == null)
        {
            Debug.Log("ステータスがありません");
        }
    }

    private void Update()
    {
        if (manager.IsInsideZone(transform.position))
        {
            outOfZoneTimer += Time.deltaTime;
            if (outOfZoneTimer > maxOutTimer)
            {
                outOfZoneTimer = 0.0f;
                status.TakeDamage(zoneDamage);
            }
        }
    }
}
