using Unity.Netcode;
using UnityEngine;

// 重要: このスクリプトが付いたオブジェクトは「シーンに配置しない」でください。
// NetworkManagerController から動的に Instantiate + Spawn することで、
// Netcode 側のシーン内オブジェクト追跡(in-scene placed object tracking)と
// 衝突しないようにし、シーン遷移をまたいでも IsServer 等が正しく機能するようにする。
public class MatchResult : NetworkBehaviour
{
    public static MatchResult Instance { get; private set; }

    // 勝者のIDを保持する。サーバーのみ書き込み可、全クライアントが読み取り可能。
    public NetworkVariable<int> WinnerId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

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

    public override void OnNetworkSpawn()
    {
        // 動的スポーンしたオブジェクトなので、ここで DestroyWithScene=false にしても
        // in-scene placed object のような追跡上の矛盾は起きない
        NetworkObject.DestroyWithScene = false;
    }
}
