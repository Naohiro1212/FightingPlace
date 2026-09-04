using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    // 現在弾数をとってくる
    [SerializeField] Weapon weapon;
    [SerializeField] TextMeshProUGUI UItext;

    private void Start()
    {
        if (weapon == null)
        {
            Debug.Log("weaponが未設定です");
        }
        if (UItext == null)
        {
            Debug.Log("テキストが未設定です");
        }
    }

    private void Update()
    {
        UItext.text = ":" + weapon.getCurrentAmmo().Value.ToString();
    }
}
