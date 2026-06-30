using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f; // 弾の速度
    private Vector3 startposition; // 生まれた場所を記憶する変数

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStatus playerStatus;

    void Start()
    {
        // 最初に生成された座標を記憶しておく
        startposition = transform.position;
        Debug.Log(startposition);
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
        Debug.Log("AddForce実行中！ 力の向き: " + (transform.up * speed));
        rb.linearVelocity = transform.up * speed;
    }

    public void SetUp(WeaponData weapondata, PlayerStatus playerstatus)
    {
        weaponData = weapondata;
        playerStatus = playerstatus;

        Debug.Log("発射者 = " + playerStatus.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStatus>(out PlayerStatus targetStatus))
        {
            targetStatus.TakeDamage(weaponData.damage);
            Destroy(gameObject);
        }
    }
}
