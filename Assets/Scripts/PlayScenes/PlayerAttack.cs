using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private WeaponData initialWeaponData;
    [SerializeField] private Weapon currentWeapon;

    private float lastAttackTime;
    private PlayerStatus playerStatus;

    private void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        playerStatus.canShoot = currentWeapon != null;

        if (initialWeaponData != null)
        {
            EquipWeapon(initialWeaponData);
        }
    }

    void Update()
    {
        if (Mouse.current == null || currentWeapon == null) return;

        bool leftClickPressed = Mouse.current.leftButton.wasPressedThisFrame;
        if (leftClickPressed && playerStatus.canShoot)
        {
            TryAttack();
        }
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (newWeaponData == null || currentWeapon == null) return;

        currentWeapon.SetUp(playerStatus, currentWeapon.FirePoint, newWeaponData);
        
        playerStatus.canShoot = true;
    }

    public void TryAttack()
    {
        if (currentWeapon == null) return;

        if (Time.time < lastAttackTime + currentWeapon.Data.cooldown)
            return;

        currentWeapon.Attack();
        lastAttackTime = Time.time;
    }
}