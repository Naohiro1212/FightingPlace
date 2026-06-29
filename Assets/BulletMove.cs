using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f; // 弾の速度
    private Vector3 startposition; // 生まれた場所を記憶する変数

    private WeaponData weaponData;
    private Rigidbody rb;

    void Start()
    {
        // 銃のステータスからとってくる
        GameObject weapon = GameObject.FindWithTag("Gun");
        weaponData = weapon.GetComponent<WeaponData>();

        // 最初に生成された座標を記憶しておく
        startposition = transform.position;

        // Rigidbodyを取得し、弾の正面に向かって一気にスピードを与える
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.up * speed;
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
        
    }
}
