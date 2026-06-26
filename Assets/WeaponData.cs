using UnityEngine;

public enum AttackType
{
    Unarmed, // 素手
    Gun, // 銃
    Melee // 近接武器
}

[CreateAssetMenu(fileName = "AttackData", menuName = "Game/Attack Data")]
public class WeaponData : ScriptableObject
{
    public string attackName; // 攻撃名
    public AttackType attackType; // 攻撃種
    public int damage; // ダメージ
    public float cooldown; // クールダウン
    public float range; // 範囲
    public int Cost;　// 1発撃つごとに消費する数

    [Header("Gun settings")]
    public int maxAmmo;
    public GameObject bulletPrefab;

    [Header("Melee / Unarmed Settings")]
    public float attackRadius;
}
