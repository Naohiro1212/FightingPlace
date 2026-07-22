using Unity.Netcode;
using UnityEngine;

public class BulletMove : NetworkBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Vector3 startPosition;

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStatus playerStatus;

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
        }
    }

    public void InitializeOnServer(WeaponData weapondata, int shooterId, Vector3 fireDirection)
    {
        weaponData = weapondata;
        shooterPlayerId = shooterId;
        direction = fireDirection.normalized;
        startPosition = transform.position;

        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 90f, 0f);
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(startPosition, transform.position);

        if (currentDistance > weaponData.range)
        {
            NetworkObject.Despawn();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    public void SetUp(WeaponData weapondata, PlayerStatus playerstatus, Vector3 fireDirection)
    {
        weaponData = weapondata;
        playerStatus = playerstatus;
        direction = fireDirection.normalized;

        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 90f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!(other is BoxCollider))
        {
            return;
        }

        if (other.TryGetComponent<PlayerStatus>(out PlayerStatus targetStatus))
        {
            if (shooterPlayerId != targetStatus.playerID)
            {
                targetStatus.TakeDamage(weaponData.damage);
                NetworkObject.Despawn();
                return;
            }
        }

        if (other.gameObject.CompareTag("StaticObject"))
        {
            NetworkObject.Despawn();
        }
    }
}
