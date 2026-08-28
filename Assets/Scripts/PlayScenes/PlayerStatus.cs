using Unity.Netcode;
using UnityEngine;

public class PlayerStatus : NetworkBehaviour
{
    public NetworkVariable<int> playerID =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    [SerializeField]
    private int maxHealth = 100;

    // =========================================
    // HPをNetworkVariableに変更
    // =========================================
    private NetworkVariable<int> currentHealth =
        new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public int MaxHealth => maxHealth;

    // HPBar側ではこれまで通り
    // status.CurrentHealth
    // と書ける
    public int CurrentHealth => currentHealth.Value;


    private NetworkVariable<bool> isDead =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<bool> canMove =
        new NetworkVariable<bool>(
            true,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<bool> canShoot =
        new NetworkVariable<bool>(
            true,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    // =========================================
    // Network Spawn
    // =========================================

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // HP初期化はServerだけ
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        // HP変更監視
        currentHealth.OnValueChanged += OnHealthChanged;

        Debug.Log(
            $"[PlayerStatus] OnNetworkSpawn " +
            $"PlayerID={playerID.Value}, " +
            $"IsServer={IsServer}, " +
            $"IsOwner={IsOwner}, " +
            $"HP={currentHealth.Value}"
        );
    }


    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;

        base.OnNetworkDespawn();
    }


    // =========================================
    // HP変更通知
    // =========================================

    private void OnHealthChanged(
        int previousValue,
        int newValue)
    {
        Debug.Log(
            $"[PlayerStatus] HP Changed " +
            $"PlayerID={playerID.Value} " +
            $"{previousValue} -> {newValue} " +
            $"IsServer={IsServer}"
        );
    }


    // =========================================
    // Dead
    // =========================================

    public bool IsDead()
    {
        return isDead.Value;
    }


    // =========================================
    // Damage
    // =========================================

    public void TakeDamage(int amount)
    {
        Debug.Log(
            $"[PlayerStatus] TakeDamage " +
            $"PlayerID={playerID.Value}, " +
            $"amount={amount}, " +
            $"IsServer={IsServer}, " +
            $"HP(before)={currentHealth.Value}"
        );

        // HP変更はServerだけ
        if (!IsServer)
        {
            return;
        }

        // 既に死亡している
        if (isDead.Value)
        {
            Debug.Log(
                $"[PlayerStatus] " +
                $"{name} is already dead"
            );

            return;
        }


        // =========================================
        // HP減少
        // =========================================

        currentHealth.Value -= amount;

        // HPがマイナスにならないようにする
        if (currentHealth.Value < 0)
        {
            currentHealth.Value = 0;
        }


        Debug.Log(
            $"[PlayerStatus] " +
            $"{name} HP(after)=" +
            $"{currentHealth.Value}"
        );


        // =========================================
        // 死亡
        // =========================================

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }


    // =========================================
    // Die
    // =========================================

    private void Die()
    {
        if (!IsServer)
        {
            return;
        }

        Debug.Log(
            $"[PlayerStatus] Die() " +
            $"PlayerID={playerID.Value}"
        );

        isDead.Value = true;

        canMove.Value = false;
        canShoot.Value = false;


        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "[PlayerStatus] " +
                "GameManager.Instance が null です"
            );

            return;
        }


        GameManager.Instance.OnPlayerDown(
            playerID.Value
        );
    }
}