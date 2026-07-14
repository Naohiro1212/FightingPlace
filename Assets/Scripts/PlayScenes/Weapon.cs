using NUnit.Framework;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform modelAnchor;
    private GameObject currentModel;

    public Transform FirePoint => firePoint;
    public WeaponData WeaponData => weaponData;

    public void SetUp(PlayerStatus playerStatus, Transform firePoint, WeaponData weaponData)
    {
        this.playerStatus = playerStatus;
        this.firePoint = firePoint;
        this.weaponData = weaponData;

        ResetAmmo();
        RefreshModel();
    }

    private void ResetAmmo()
    {
        if (weaponData != null && weaponData.attackType == AttackType.Gun)
        {
            currentAmmo = weaponData.maxAmmo;
        }
        else
        {
            currentAmmo = 0;
        }
    }

    private void RefreshModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        if (weaponData != null && weaponData.GunPrefab != null && modelAnchor != null)
        {
            currentModel = Instantiate(weaponData.GunPrefab, modelAnchor);
            currentModel.transform.localPosition = Vector3.zero;
            currentModel.transform.localRotation = Quaternion.identity;
        }
    }

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
            Vector3 fireDirection = playerStatus.transform.forward.normalized;

            Vector3 spawnPosition = firePoint.position + fireDirection * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(fireDirection);

            GameObject bullet = Instantiate(weaponData.bulletPrefab, spawnPosition, rotation);

            BulletMove bulletMove = bullet.GetComponent<BulletMove>();
            if (bulletMove != null)
            {
                bulletMove.SetUp(weaponData, playerStatus, fireDirection);
            }
        }
    }
}