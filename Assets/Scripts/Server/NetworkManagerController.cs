using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManagerController : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private string hostIpAddress = "192.168.56.1";
    [SerializeField] private ushort port = 7777;

    [Header("Scene Settings")]
    [SerializeField] private string battleSceneName = "SampleScene";
    [SerializeField] private int requiredPlayerCount = 2;

    [Header("Match Result")]
    [SerializeField] private GameObject matchResultPrefab;

    // ==============================
    // 接続状態表示
    // ==============================
    [Header("Connection UI")]
    [SerializeField] private TextMeshProUGUI connectionStatusText;

    private bool matchResultSpawned = false;
    private bool hasStarted = false;
    private bool hasRequestedSceneLoad = false;
    private bool callbacksRegistered = false;

    private string hostIpAddressInput = string.Empty;

    // TMPとOnGUIの両方で使用
    private string statusMessage = "not connecting";

    private NetworkManager networkManager;

    private static NetworkManagerController instance;

    public static NetworkManagerController Instance => instance;

    // ============================================================
    // Unity
    // ============================================================

    private void Awake()
    {
        // LobbySceneに戻ってきた時の重複防止
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        hostIpAddressInput = hostIpAddress;

        DontDestroyOnLoad(gameObject);
    
    }


    private void Start()
    {
        RegisterNetworkCallbacks();

        // 最初は未接続
        SetConnectionStatus("not connecting");
    }


    private void OnDestroy()
    {
        UnregisterNetworkCallbacks();

        if (instance == this)
        {
            instance = null;
        }
    }


    // ============================================================
    // 接続状態UI
    // ============================================================

    private void SetConnectionStatus(string message)
    {
        statusMessage = message;

        if (connectionStatusText != null)
        {
            connectionStatusText.text = message;
        }

        Debug.Log(
            $"[ConnectionStatus] {message}"
        );
    }

    public void SetConnectionStatusText(TextMeshProUGUI text)
    {
        connectionStatusText = text;

        if (connectionStatusText != null)
        {
            connectionStatusText.text = statusMessage;
        }
    }


    // ============================================================
    // NetworkManager 初期化
    // ============================================================

    private bool GetNetworkManager()
    {
        if (networkManager != null)
        {
            return true;
        }

        networkManager = NetworkManager.Singleton;

        if (networkManager == null)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "NetworkManager.Singleton がありません"
            );

            return false;
        }

        return true;
    }


    // ============================================================
    // Callback登録
    // ============================================================

    private void RegisterNetworkCallbacks()
    {
        if (callbacksRegistered)
        {
            return;
        }

        if (!GetNetworkManager())
        {
            return;
        }

        // LobbyではPlayerObjectを自動生成しない
        networkManager.NetworkConfig.ConnectionApproval = true;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;

        networkManager.OnClientConnectedCallback +=
            OnClientConnected;

        networkManager.OnClientDisconnectCallback +=
            OnClientDisconnected;

        callbacksRegistered = true;

        Debug.Log(
            "[NetworkManagerController] " +
            "Network Callbackを登録しました"
        );
    }


    private void UnregisterNetworkCallbacks()
    {
        if (!callbacksRegistered)
        {
            return;
        }

        if (networkManager == null)
        {
            return;
        }

        networkManager.ConnectionApprovalCallback -=
            ApprovalCheck;

        networkManager.OnClientConnectedCallback -=
            OnClientConnected;

        networkManager.OnClientDisconnectCallback -=
            OnClientDisconnected;

        callbacksRegistered = false;
    }


    // ============================================================
    // Connection Approval
    // ============================================================

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log(
            $"[ConnectionApproval] " +
            $"ClientId={request.ClientNetworkId}"
        );

        // 接続許可
        response.Approved = true;

        // LobbyではPlayerを生成しない
        response.CreatePlayerObject = false;

        response.Pending = false;
    }


    // ============================================================
    // GUI
    // ============================================================

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(10, 10, 320, 300)
        );

        GUILayout.Label("接続先IPアドレス");

        hostIpAddressInput =
            GUILayout.TextField(hostIpAddressInput);

        GUILayout.Space(10);

        GUILayout.Label(
            $"IP: {hostIpAddress}"
        );

        GUILayout.Label(
            $"Port: {port}"
        );

        GUILayout.Label(
            $"Status: {statusMessage}"
        );

        GUILayout.EndArea();
    }


    // ============================================================
    // Matchmakingから呼ぶ
    // ============================================================

    public void StartMatchedHost(
        string sceneName,
        ushort matchedPort)
    {
        battleSceneName = sceneName;
        port = matchedPort;

        EnsureMatchSession().ConfigureAsHost(
            battleSceneName,
            port
        );

        StartAsHost();
    }


    public void StartMatchedClient(
        string matchedIpAddress,
        string sceneName,
        ushort matchedPort)
    {
        hostIpAddress = matchedIpAddress;
        hostIpAddressInput = matchedIpAddress;

        battleSceneName = sceneName;
        port = matchedPort;

        EnsureMatchSession().ConfigureAsClient(
            hostIpAddress,
            port,
            battleSceneName
        );

        StartAsClient();
    }


    // ============================================================
    // Host
    // ============================================================

    public void StartAsHost()
    {
        if (!GetNetworkManager())
        {
            return;
        }

        RegisterNetworkCallbacks();

        if (hasStarted || networkManager.IsListening)
        {
            Debug.LogWarning(
                "[NetworkManagerController] " +
                "NetworkManagerは既に起動しています"
            );

            return;
        }

        UnityTransport transport =
            networkManager.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "UnityTransport が見つかりません"
            );

            return;
        }

        // Hostは全NICで待ち受ける
        transport.SetConnectionData(
            hostIpAddress,
            port,
            "0.0.0.0"
        );

        // 接続開始
        SetConnectionStatus("connecting");

        bool result =
            networkManager.StartHost();

        if (!result)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "StartHost() に失敗しました"
            );

            SetConnectionStatus("not connecting");

            return;
        }

        hasStarted = true;

        Debug.Log(
            $"[NetworkManagerController] " +
            $"Host Started " +
            $"IP={hostIpAddress} Port={port}"
        );
    }


    // ============================================================
    // Client
    // ============================================================

    public void StartAsClient()
    {
        if (!GetNetworkManager())
        {
            return;
        }

        RegisterNetworkCallbacks();

        if (hasStarted || networkManager.IsListening)
        {
            Debug.LogWarning(
                "[NetworkManagerController] " +
                "NetworkManagerは既に起動しています"
            );

            return;
        }

        if (!string.IsNullOrWhiteSpace(hostIpAddressInput))
        {
            hostIpAddress =
                hostIpAddressInput.Trim();
        }

        UnityTransport transport =
            networkManager.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "UnityTransport が見つかりません"
            );

            return;
        }

        transport.SetConnectionData(
            hostIpAddress,
            port
        );

        // =====================================
        // Client接続開始
        // =====================================
        SetConnectionStatus("connecting");

        bool result =
            networkManager.StartClient();

        if (!result)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "StartClient() に失敗しました"
            );

            SetConnectionStatus("not connecting");

            return;
        }

        hasStarted = true;

        Debug.Log(
            $"[NetworkManagerController] " +
            $"Client Started " +
            $"IP={hostIpAddress} Port={port}"
        );
    }


    // ============================================================
    // Dedicated Server
    // ============================================================

    public void StartAsServer()
    {
        if (!GetNetworkManager())
        {
            return;
        }

        RegisterNetworkCallbacks();

        if (hasStarted || networkManager.IsListening)
        {
            Debug.LogWarning(
                "[NetworkManagerController] " +
                "NetworkManagerは既に起動しています"
            );

            return;
        }

        UnityTransport transport =
            networkManager.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "UnityTransport が見つかりません"
            );

            return;
        }

        transport.SetConnectionData(
            hostIpAddress,
            port,
            "0.0.0.0"
        );

        SetConnectionStatus("connecting");

        bool result =
            networkManager.StartServer();

        if (!result)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "StartServer() に失敗しました"
            );

            SetConnectionStatus("not connecting");

            return;
        }

        hasStarted = true;

        Debug.Log(
            $"[NetworkManagerController] " +
            $"Server Started Port={port}"
        );
    }


    // ============================================================
    // Client接続
    // ============================================================

    private void OnClientConnected(ulong clientId)
    {
        if (!GetNetworkManager())
        {
            return;
        }

        Debug.Log(
            $"[NetworkManagerController] " +
            $"Client Connected " +
            $"ClientId={clientId}, " +
            $"ConnectedCount=" +
            $"{networkManager.ConnectedClientsIds.Count}"
        );


        // ========================================================
        // Client側
        // ========================================================

        if (!networkManager.IsServer)
        {
            // Serverへの接続成功
            SetConnectionStatus("connecting complete");

            return;
        }


        // ========================================================
        // Host / Server側
        // ========================================================

        DumpSpawnedNetworkObjects();


        // 必要人数に達していない
        if (networkManager.ConnectedClientsIds.Count
            < requiredPlayerCount)
        {
            SetConnectionStatus("connecting");

            Debug.Log(
                $"[NetworkManagerController] " +
                $"プレイヤー待機中 " +
                $"{networkManager.ConnectedClientsIds.Count}" +
                $"/{requiredPlayerCount}"
            );

            return;
        }


        // =====================================
        // 必要人数が揃った
        // =====================================

        SetConnectionStatus("connecting complete");


        if (hasRequestedSceneLoad)
        {
            return;
        }

        hasRequestedSceneLoad = true;


        // 全Client接続後にMatchResult生成
        SpawnMatchResultIfNeeded();


        Debug.Log(
            $"[NetworkManagerController] " +
            $"{requiredPlayerCount}人接続したため " +
            $"{battleSceneName} へ移動します"
        );


        networkManager.SceneManager.LoadScene(
            battleSceneName,
            LoadSceneMode.Single
        );
    }


    // ============================================================
    // Client切断
    // ============================================================

    private void OnClientDisconnected(ulong clientId)
    {
        if (networkManager == null)
        {
            return;
        }

        Debug.Log(
            $"[NetworkManagerController] " +
            $"Client Disconnected " +
            $"ClientId={clientId}"
        );


        // ========================================================
        // Client自身がServerから切断された
        // ========================================================

        if (!networkManager.IsServer)
        {
            SetConnectionStatus("not connecting");

            hasStarted = false;
            hasRequestedSceneLoad = false;

            return;
        }


        // ========================================================
        // Host / Server側で誰かが抜けた
        // ========================================================

        if (!hasRequestedSceneLoad)
        {
            if (networkManager.ConnectedClientsIds.Count
                < requiredPlayerCount)
            {
                SetConnectionStatus("connecting");
            }
        }
    }


    // ============================================================
    // MatchResult
    // ============================================================

    private void SpawnMatchResultIfNeeded()
    {
        if (!GetNetworkManager())
        {
            return;
        }

        if (matchResultSpawned)
        {
            return;
        }

        if (!networkManager.IsServer)
        {
            return;
        }

        if (matchResultPrefab == null)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "matchResultPrefab が設定されていません"
            );

            return;
        }

        GameObject go =
            Instantiate(matchResultPrefab);

        NetworkObject netObj =
            go.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError(
                "[NetworkManagerController] " +
                "MatchResult PrefabにNetworkObjectがありません"
            );

            Destroy(go);

            return;
        }

        // Scene切り替えでも破棄しない
        netObj.Spawn(false);

        matchResultSpawned = true;

        Debug.Log(
            "[NetworkManagerController] " +
            "MatchResultをSpawnしました"
        );
    }


    // ============================================================
    // Debug
    // ============================================================

    private void DumpSpawnedNetworkObjects()
    {
        if (!GetNetworkManager())
        {
            return;
        }

        if (networkManager.SpawnManager == null)
        {
            Debug.LogWarning(
                "[NET CHECK] SpawnManager がありません"
            );

            return;
        }

        Debug.Log(
            "========== Spawned NetworkObjects =========="
        );

        foreach (
            NetworkObject netObj
            in networkManager.SpawnManager.SpawnedObjectsList)
        {
            if (netObj == null)
            {
                Debug.LogError(
                    "[NET CHECK] " +
                    "SpawnedObjectsList にnullがあります"
                );

                continue;
            }

            Debug.Log(
                $"[NET CHECK] " +
                $"Name={netObj.name}, " +
                $"ID={netObj.NetworkObjectId}, " +
                $"Scene={netObj.gameObject.scene.name}, " +
                $"IsSpawned={netObj.IsSpawned}, " +
                $"IsPlayer={netObj.IsPlayerObject}, " +
                $"Owner={netObj.OwnerClientId}, " +
                $"Active={netObj.gameObject.activeInHierarchy}"
            );
        }

        Debug.Log(
            "============================================"
        );
    }


    // ============================================================
    // MatchSession
    // ============================================================

    private MatchSession EnsureMatchSession()
    {
        if (MatchSession.Instance != null)
        {
            return MatchSession.Instance;
        }

        GameObject go =
            new GameObject(nameof(MatchSession));

        return go.AddComponent<MatchSession>();
    }


    // ============================================================
    // GameEnd
    // ============================================================

    public void GameEnd()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }

    public void Disconnect()
    {
        if (networkManager != null)
        {
            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }

        hasStarted = false;
        hasRequestedSceneLoad = false;
        matchResultSpawned = false;

        SetConnectionStatus("not connecting");

        Debug.Log(
            "[NetworkManagerController] 通信状態をリセットしました"
        );
    }
}