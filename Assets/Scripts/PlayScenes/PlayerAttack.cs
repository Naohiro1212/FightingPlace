using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;

    private float lastAttackTime;
    private PlayerStatus playerStatus;

    private void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        // 開始時は攻撃できない状態にする
        playerStatus.canShoot = false;
    }

    void Update()
    {
        // 仮でマウスの左クリックで攻撃するようにしている。実際ではコントローラー
        bool LeftClickPressed = Mouse.current.leftButton.wasPressedThisFrame;
        if (Mouse.current != null && LeftClickPressed && playerStatus.canShoot)
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