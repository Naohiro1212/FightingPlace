using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int playerID; // 1 or 2 をインスペクターで設定
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int MaxHealth => maxHealth;

    private bool isDead = false;
    public bool canMove = true;
    public bool canShoot = true;

    public bool IsDead()
    {
        return isDead;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    // ダメージを受けた時にだけ勝敗判定を行う
    public void TakeDamage(int amount)
    {
        // 既に死んでいたら、これ以上ダメージ処理をしない
        if (isDead) return;

        currentHealth -= amount;

        // もしダメージを受けた結果、HPが0以下になったら
        if (currentHealth <= 0)
        {
            currentHealth = 0; // HPがマイナスにならないようにする（見栄え用）
            Die();             // 死亡処理を呼ぶ
        }
    }

    private void Die()
    {
        isDead = true;
        canMove = false;
        canShoot = false;

        // ここなら「死んだ瞬間」の1回しか呼ばれないので安全！
        GameManager.Instance.OnPlayerDown(playerID);
    }
}