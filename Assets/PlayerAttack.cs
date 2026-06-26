using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;

    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.Attack();
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
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