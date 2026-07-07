using Unity.Mathematics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Transform firePoint;

    [SerializeField] private int currentAmmo;

    public WeaponData Data => weaponData;
    public float Cooldown => weaponData.cooldown;

    private void Awake()
    {
        if (weaponData != null && weaponData.attackType == AttackType.Gun)
        {
            currentAmmo = weaponData.maxAmmo;
        }
    }

    public void Attack()
    {
        if (weaponData == null) return;

        switch (weaponData.attackType)
        {
            case AttackType.Unarmed:
            case AttackType.Melee:
                DoMeleeAttack();
                break;

            case AttackType.Gun:
                DoGunAttack();
                break;
        }
    }

    private void DoMeleeAttack()
    {
        Debug.Log($"{weaponData.attackName} で近接攻撃");
    }

    private void DoGunAttack()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("弾切れ");
            return;
        }

        currentAmmo--;
        Debug.Log($"{weaponData.attackName} 発射");

        if (weaponData.bulletPrefab != null && firePoint != null)
        {
            // 銃口の回転に対して、X軸に-90度回転を加えて生成
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(90, 0, 0);

            // 位置は銃口の少し前方に設定
            Vector3 spawnPosition = firePoint.position + firePoint.forward * 0.5f;

            // 弾を生成し、GameObjectとして変数に受ける
            GameObject bullet = Instantiate(weaponData.bulletPrefab, spawnPosition, rotation);

            Debug.Log("Gun位置: " + transform.position);

            // 弾についているBulletMoveスクリプトを取得する
            BulletMove bulletMove = bullet.GetComponent<BulletMove>();

            if (bulletMove != null)
            {
                // 弾のSetupメソッドを読んで、自分のWeaponDataとplayerStatusを渡す
                bulletMove.SetUp(weaponData, playerStatus);
            }
        }
    }
}