using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class NetworkPlayerContext : NetworkBehaviour
{
    [SerializeField] private Behaviour[] ownerOnlyBehaviours;
    [SerializeField] private PlayerStatus playerStatus;

    private readonly NetworkVariable<int> playerId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        SetOwnerOnlyBehaviours(IsOwner);
        ApplyPlayerId(playerId.Value);
        playerId.OnValueChanged += OnPlayerIdChanged;
    }

    public override void OnNetworkDespawn()
    {
        playerId.OnValueChanged -= OnPlayerIdChanged;
    }

    public void SetPlayerId(int value)
    {
        if (!IsServer) return;
        playerId.Value = value;
        ApplyPlayerId(value);
    }

    private void OnPlayerIdChanged(int previousValue, int newValue)
    {
        ApplyPlayerId(newValue);
    }

    private void ApplyPlayerId(int value)
    {
        if(playerStatus != null)
        {
            playerStatus.playerID.Value = value;
        }
    }

    private void SetOwnerOnlyBehaviours(bool enabledValue)
    {
        if (ownerOnlyBehaviours == null) return;

        for (int i = 0; i < ownerOnlyBehaviours.Length; i++)
        {
            if (ownerOnlyBehaviours[i] != null)
            {
                ownerOnlyBehaviours[i].enabled = enabledValue;
            }
        }
    }
}
