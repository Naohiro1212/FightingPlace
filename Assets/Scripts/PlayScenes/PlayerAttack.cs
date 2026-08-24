using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private WeaponData initialWeaponData;
    [SerializeField] private Weapon currentWeapon;

    private float lastAttackTime;
    private PlayerStatus playerStatus;

    public override void OnNetworkSpawn()
    {
        if (playerStatus == null)
        {
            return;
        }

        playerStatus.canShoot.Value = currentWeapon != null;

        if (initialWeaponData != null && currentWeapon != null)
        {
            EquipWeapon(initialWeaponData);
        }
    }

    private void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        playerStatus.canShoot.Value = currentWeapon != null;

        if (initialWeaponData != null)
        {
            EquipWeapon(initialWeaponData);
        }
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner)
        {
            return;
        }

        if (Mouse.current == null || currentWeapon == null || playerStatus == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && playerStatus.canShoot.Value)
        {
            TryAttack();
        }
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (newWeaponData == null || currentWeapon == null || playerStatus == null)
        {
            return;
        }

        currentWeapon.SetUp(playerStatus, currentWeapon.FirePoint, newWeaponData);
        playerStatus.canShoot.Value = true;
    }

    public void TryAttack()
    {
        if (currentWeapon == null || currentWeapon.Data == null)
        {
            return;
        }

        if (Time.time < lastAttackTime + currentWeapon.Data.cooldown)
        {
            return;
        }

        currentWeapon.Attack();
        lastAttackTime = Time.time;
    }
}