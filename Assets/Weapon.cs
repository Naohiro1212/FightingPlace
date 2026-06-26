using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;

    private int currentAmmo;

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
            Instantiate(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}