using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerController : MonoBehaviour
{
    [SerializeField] private string hostIpAddress = "192.168.42.16";
    [SerializeField] private ushort port = 7777;
    private bool hasStarted = false;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

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

        GUILayout.EndArea();
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
    }
}
