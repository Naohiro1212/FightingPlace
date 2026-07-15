using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Vector3 startposition;

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStatus playerStatus;

    private Vector3 direction;

    void Start()
    {
        startposition = transform.position;
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(startposition, transform.position);

        if (currentDistance > weaponData.range)
        {
            Destroy(gameObject);
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
        if (!(other is BoxCollider))
        {
            return;
        }

        if (other.TryGetComponent<PlayerStatus>(out PlayerStatus targetStatus))
        {
            if (playerStatus.playerID != targetStatus.playerID)
            {
                targetStatus.TakeDamage(weaponData.damage);
                Destroy(gameObject);
            }
        }

        if (other.gameObject.CompareTag("StaticObject"))
        {
            Destroy(gameObject);
        }
    }
}
