using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkBattleManager : NetworkBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private NetworkObject playerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;

    private bool hasSpawnedPlayers = false;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            return;
        }

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "[NetworkBattleManager] NetworkManager.Singleton がありません"
            );

            return;
        }

        // Scene全体のロード完了を待つ
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }


    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        }

        base.OnNetworkDespawn();
    }


    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (!IsServer)
        {
            return;
        }

        if (hasSpawnedPlayers)
        {
            return;
        }

        // Host / Client 全員のSceneロード完了
        if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted)
        {
            return;
        }

        // このManagerが存在するScene以外は無視
        if (sceneEvent.SceneName != gameObject.scene.name)
        {
            return;
        }

        Debug.Log(
            $"[NetworkBattleManager] " +
            $"Scene Load Completed: {sceneEvent.SceneName}"
        );

        SpawnPlayers();
    }


    private void SpawnPlayers()
    {
        if (hasSpawnedPlayers)
        {
            return;
        }

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "[NetworkBattleManager] NetworkManager がありません"
            );

            return;
        }

        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }


        // ==========================================
        // PlayerPrefab確認
        // ==========================================

        if (playerPrefab == null)
        {
            Debug.LogError(
                "[NetworkBattleManager] playerPrefab が設定されていません"
            );

            return;
        }


        // ==========================================
        // SpawnPoint確認
        // ==========================================

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError(
                "[NetworkBattleManager] spawnPoints が設定されていません"
            );

            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null)
            {
                Debug.LogError(
                    $"[NetworkBattleManager] " +
                    $"spawnPoints[{i}] がnullです"
                );

                return;
            }
        }


        // ==========================================
        // 接続Client取得
        // ==========================================

        List<ulong> clientIds =
            new List<ulong>(
                NetworkManager.Singleton.ConnectedClientsIds
            );

        // ClientId順に並べる
        clientIds.Sort();


        Debug.Log(
            $"[NetworkBattleManager] " +
            $"ConnectedClients={clientIds.Count}"
        );


        if (clientIds.Count == 0)
        {
            Debug.LogError(
                "[NetworkBattleManager] 接続Clientがいません"
            );

            return;
        }


        // ==========================================
        // 使用可能なSpawnPointを作成
        // ==========================================

        List<Transform> availableSpawnPoints =
            new List<Transform>(spawnPoints);


        // ==========================================
        // 生成人数
        // ==========================================

        int playerCount =
            Mathf.Min(
                clientIds.Count,
                availableSpawnPoints.Count
            );


        // ==========================================
        // Player生成
        // ==========================================

        for (int i = 0; i < playerCount; i++)
        {
            ulong clientId = clientIds[i];


            // --------------------------------------
            // Client取得
            // --------------------------------------

            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                Debug.LogError(
                    $"[NetworkBattleManager] " +
                    $"ClientId={clientId} を取得できません"
                );

                continue;
            }


            // --------------------------------------
            // 既にPlayerObjectがあるか確認
            // --------------------------------------

            if (client.PlayerObject != null)
            {
                Debug.LogWarning(
                    $"[NetworkBattleManager] " +
                    $"ClientId={clientId} は既にPlayerObjectを持っています"
                );

                continue;
            }


            // ======================================
            // SpawnPointをランダム選択
            // ======================================

            int randomIndex =
                Random.Range(
                    0,
                    availableSpawnPoints.Count
                );

            Transform spawnPoint =
                availableSpawnPoints[randomIndex];


            // 選ばれたSpawnPointを削除
            // → 次のPlayerと重複しない
            availableSpawnPoints.RemoveAt(
                randomIndex
            );


            // ======================================
            // Spawn位置
            // ======================================

            Vector3 spawnPosition =
                spawnPoint.position +
                Vector3.up * 1.5f;

            Quaternion spawnRotation =
                spawnPoint.rotation;


            // ======================================
            // Player生成
            // ======================================

            NetworkObject player =
                Instantiate(
                    playerPrefab,
                    spawnPosition,
                    spawnRotation
                );


            if (player == null)
            {
                Debug.LogError(
                    $"[NetworkBattleManager] " +
                    $"ClientId={clientId} のPlayer生成に失敗しました"
                );

                continue;
            }


            // Client専用のPlayerObjectとしてSpawn
            player.SpawnAsPlayerObject(
                clientId,
                true
            );


            // ======================================
            // Player ID設定
            // ======================================

            int assignedPlayerId = i + 1;

            NetworkPlayerContext context =
                player.GetComponent<NetworkPlayerContext>();

            if (context != null)
            {
                context.SetPlayerId(assignedPlayerId);
            }

            PlayerStatus playerStatus =
                player.GetComponent<PlayerStatus>();

            if (playerStatus != null)
            {
                playerStatus.playerID.Value =
                    assignedPlayerId;
            }
            else
            {
                Debug.LogWarning(
                    "[NetworkBattleManager] " +
                    "PlayerStatusがありません"
                );
            }

            Debug.Log(
                $"[NetworkBattleManager] " +
                $"Player生成完了 " +
                $"ClientId={clientId}, " +
                $"PlayerId={i + 1}, " +
                $"SpawnPoint={spawnPoint.name}, " +
                $"Position={spawnPosition}"
            );
        }


        // ==========================================
        // Spawn完了
        // ==========================================

        hasSpawnedPlayers = true;


        Debug.Log(
            $"[NetworkBattleManager] " +
            $"{playerCount}人のPlayer生成完了"
        );
    }
}