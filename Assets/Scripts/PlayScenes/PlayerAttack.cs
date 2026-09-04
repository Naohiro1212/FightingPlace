using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField]
    private WeaponData initialWeaponData;

    [SerializeField]
    private Weapon currentWeapon;

    private float lastAttackTime;

    private PlayerStatus playerStatus;

    private void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();

        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatusがありません");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (playerStatus == null)
        {
            return;
        }

        if (initialWeaponData != null &&
            currentWeapon != null)
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

        if (Gamepad.current == null ||
            currentWeapon == null ||
            playerStatus == null)
        {
            return;
        }

        if (Gamepad.current.rightTrigger.isPressed &&
            playerStatus.canShoot.Value)
        {
            TryAttack();
        }
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (newWeaponData == null ||
            currentWeapon == null ||
            playerStatus == null)
        {
            return;
        }

        currentWeapon.SetUp(
            playerStatus,
            currentWeapon.FirePoint,
            newWeaponData
        );
    }

    public void TryAttack()
    {
        if (currentWeapon == null ||
            currentWeapon.Data == null)
        {
            return;
        }

        if (Time.time <
            lastAttackTime +
            currentWeapon.Data.cooldown)
        {
            return;
        }

        currentWeapon.Attack();

        lastAttackTime = Time.time;
    }
}