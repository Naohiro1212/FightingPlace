using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkBattleManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private bool hasSpawnedPlayers = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer || hasSpawnedPlayers) return;

        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (playerPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("playerPrefab Ç‹ÇΩÇÕ spawnPoints ÇÃê›íËÇ™ïsë´ÇµÇƒÇ¢Ç‹Ç∑");
            return;
        }

        var clientIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
        clientIds.Sort();

        for (int i = 0; i < clientIds.Count && i < spawnPoints.Length; i++)
        {
            ulong clientId = clientIds[i];
            var client = NetworkManager.Singleton.ConnectedClients[clientId];

            if (client.PlayerObject != null)
            {
                continue;
            }

            Transform spawnPoint = spawnPoints[i];
            NetworkObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            player.SpawnAsPlayerObject(clientId, true);

            var context = player.GetComponent<NetworkPlayerContext>();
            if (context != null)
            {
                context.SetPlayerId(i + 1);
            }
        }

        hasSpawnedPlayers = true;
    }
}
