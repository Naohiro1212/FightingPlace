using UnityEngine;

// 武器交換用のスクリプト
public class WeaponSwitcher : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("衝突");
        // 衝突したオブジェクトが武器であるかを確認
        Weapon weapon = other.gameObject.GetComponent<Weapon>();
        if (weapon != null && other.gameObject.CompareTag("Gun"))
        {
            Debug.Log("武器枠はある");
            // 武器を装備する処理を呼び出す
            PlayerAttack playerAttack = GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.EquipWeapon(weapon.WeaponData);
                Debug.Log($"武器 {weapon.Data.attackName} を装備しました。");
            }
        }
    }

}
