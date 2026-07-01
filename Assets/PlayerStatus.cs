using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerStatus : MonoBehaviour
{
    public int playerID; // 1 or 2 をインスペクターで設定
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private int currentHealth;

    public int MaxHealth => maxHealth;

    private bool isDead = false;
    public bool canMove = true;

    public bool IsDead()
    {
        return isDead;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        Dead();
    }

    private void Dead()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            canMove = false;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }
}