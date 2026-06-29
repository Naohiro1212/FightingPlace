using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;

    private float lastAttackTime;

    void Update()
    {
        // 仮でマウスの左クリックで攻撃するようにしている。実際ではコントローラー
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
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