using UnityEngine;

public class MatchSession : MonoBehaviour
{
    public static MatchSession Instance { get; private set; }

    public string BattleSceneName { get; private set; } = "SampleScene";
    public string HostIpAddress { get; private set; } = "192.168.42.16";
    public ushort Port { get; private set; } = 7777;
    public int LocalPlayerSlot { get; private set; } = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ConfigureAsHost(string battleSceneName, ushort port)
    {
        BattleSceneName = battleSceneName;
        Port = port;
        LocalPlayerSlot = 1;
    }

    public void ConfigureAsClient(string hostIpAddress, ushort port, string battleSceneName)
    {
        HostIpAddress = hostIpAddress;
        Port = port;
        BattleSceneName = battleSceneName;
        LocalPlayerSlot = 2;
    }
}
