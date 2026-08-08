using Unity.Netcode;
using UnityEngine;

public class BulletMove : NetworkBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Vector3 startPosition;

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Rigidbody rb;

    private Vector3 direction;
    private int shooterPlayerId;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (IsServer)
        {
            startPosition = transform.position;
            Debug.Log($"[Bullet] Spawned on Server: {name}");
        }
    }

    public void InitializeOnServer(WeaponData weapondata, int shooterId, Vector3 fireDirection)
    {
        weaponData = weapondata;
        shooterPlayerId = shooterId;
        direction = fireDirection.normalized;
        startPosition = transform.position;

        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 90f, 0f);

        Debug.Log($"[Bullet] Init shooterPlayerId={shooterPlayerId}, direction={direction}");
    }

    private void Update()
    {
        if (!IsServer || weaponData == null)
        {
            return;
        }

        float currentDistance = Vector3.Distance(startPosition, transform.position);
        if (currentDistance > weaponData.range)
        {
            Debug.Log("[Bullet] Range over -> Despawn");
            NetworkObject.Despawn();
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer || rb == null)
        {
            return;
        }

        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Bullet] Trigger with: {other.name}");
        Debug.Log($"[Bullet] tag={other.tag}, layer={LayerMask.LayerToName(other.gameObject.layer)}, root={other.transform.root.name}");

        PlayerStatus targetStatus = other.GetComponentInParent<PlayerStatus>();
        Debug.Log($"[Bullet] targetStatus={(targetStatus != null ? targetStatus.name : "null")}");

        if (!IsServer || weaponData == null)
        {
            return;
        }

        if (targetStatus != null)
        {
            if (shooterPlayerId != targetStatus.playerID)
            {
                targetStatus.TakeDamage(weaponData.damage);
                Debug.Log("[Bullet] Damage applied -> Despawn");
                NetworkObject.Despawn();
            }

            return;
        }

        if (other.gameObject.CompareTag("StaticObject"))
        {
            Debug.Log("[Bullet] Hit StaticObject -> Despawn");
            NetworkObject.Despawn();
        }
    }
}
