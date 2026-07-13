using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f; // 弾の速度
    private Vector3 startposition; // 生まれた場所を記憶する変数

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStatus playerStatus;

    private Vector3 direction; // 進行方向を保持

    void Start()
    {
        // 最初に生成された座標を記憶しておく
        startposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 1.生成された座標から、今の座標までの計算をする
        float currentDistance = Vector3.Distance(startposition, transform.position);

        // 2.もし設定した最大距離を超えていたら、自分自身を消去する
        //   銃の場合はRangeをそのまま最大距離として扱う
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

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        // BoxColliderのみ判定を行う
        if (!(other is BoxCollider))
        {
            return;
        }

        // 敵との当たり判定
        if (other.TryGetComponent<PlayerStatus>(out PlayerStatus targetStatus))
        {
            // 自プレイヤーに当たってダメージを与えないようにする
            if(playerStatus.playerID != targetStatus.playerID)
            {
                targetStatus.TakeDamage(weaponData.damage);
                Destroy(gameObject);
            }
        }

        // 静的なオブジェクトに当たった場合も弾を消す
        if (other.gameObject.CompareTag("StaticObject"))
        {
            Destroy(gameObject);
        }
    }
}
