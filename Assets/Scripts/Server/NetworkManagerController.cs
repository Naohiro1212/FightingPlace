using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerController : MonoBehaviour
{
    [SerializeField] private string hostIpAddress = "192.168.42.16";
    [SerializeField] private ushort port = 7777;
    [SerializeField] private string battleSceneName = "SampleScene";
    [SerializeField] private int requiredPlayerCount = 2;

    private bool hasStarted = false;
    private bool hasRequestedSceneLoad = false;

    private string hostIpAddressInput = string.Empty;
    private string statusMessage = string.Empty;

    private string sceneName = "";

    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        hostIpAddressInput = hostIpAddress;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        GUILayout.Label("接続先IPアドレス");
        hostIpAddressInput = GUILayout.TextField(hostIpAddressInput);

        GUILayout.Space(10);

        if (GUILayout.Button("Auto Start"))
        {
            StartByPlayerCount();
        }

        if (GUILayout.Button("Host"))
        {
            StartAsHost();
        }

        if (GUILayout.Button("Client"))
        {
            StartAsClient();
        }

        if (GUILayout.Button("Server"))
        {
            NetworkManager.Singleton.StartServer();
        }

        GUILayout.Label($"IP: {hostIpAddress}");
        GUILayout.Label($"Port: {port}");

        GUILayout.EndArea();
    }

    public void StartMatchedHost(string sceneName, ushort matchedPort)
    {
        battleSceneName = sceneName;
        port = matchedPort;

        EnsureMatchSession().ConfigureAsHost(battleSceneName, port);
        StartAsHost();
    }

    public void StartMatchedClient(string matchedIpAddress, string sceneName, ushort matchedPort)
    {
        hostIpAddress = matchedIpAddress;
        battleSceneName = sceneName;
        port = matchedPort;

        EnsureMatchSession().ConfigureAsClient(hostIpAddress, port, battleSceneName);
        StartAsClient();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        if (hasRequestedSceneLoad)
        {
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count < requiredPlayerCount)
        {
            return;
        }

        hasRequestedSceneLoad = true;
        NetworkManager.Singleton.SceneManager.LoadScene(battleSceneName, LoadSceneMode.Single);
    }

    private void StartByPlayerCount()
    {
        var players = FindObjectsByType<PlayerStatus>();

        if (players.Length == 0)
        {
            StartAsHost();
            return;
        }

        StartAsClient();
    }

    private void StartAsHost()
    {
        if (hasStarted) return;

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport が見つかりません");
            return;
        }

        transport.SetConnectionData(hostIpAddress, port);
        NetworkManager.Singleton.StartHost();
        hasStarted = true;
    }

    private void StartAsClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport が見つかりません");
            return;
        }

        transport.SetConnectionData(hostIpAddress, port);
        NetworkManager.Singleton.StartClient();
        hasStarted = true;
    }

    private MatchSession EnsureMatchSession()
    {
        if(MatchSession.Instance != null)
        {
            return MatchSession.Instance;
        }

        var go = new GameObject(nameof(MatchSession));
        return go.AddComponent<MatchSession>();
    }

    private void ChangeScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
