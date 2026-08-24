using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] players;

    private int minPoint = 2;

    private void Start()
    {
        SearchPlayers();
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        // 生成位置の数の担保
        if (spawnPoints.Length < minPoint) return;

        // playerID の割り当てと位置決めはサーバーのみが行う。
        // NetworkVariable の書き込みも Server 権限のみ許可されているため、
        // クライアント側でこのメソッドを実行すると playerID.Value の代入で例外になる。
        // 位置(Transform)についても、PlayerオブジェクトにNetworkTransformが付いていれば
        // サーバー側の変更が自動的に全クライアントへ同期される。
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        int index1 = Random.Range(0, spawnPoints.Length);
        int index2;

        // 生成位置が同じにならないように
        do
        {
            index2 = Random.Range(0, spawnPoints.Length);
        }
        while (index1 == index2);

        // 埋まってしまわないように想定した位置より少し上に生成
        players[0].transform.position = spawnPoints[index1].position + Vector3.up * 1.5f;
        players[1].transform.position = spawnPoints[index2].position + Vector3.up * 1.5f;

        // プレイヤーIDを設定（NetworkVariable、サーバーのみ書き込み可）
        players[0].GetComponent<PlayerStatus>().playerID.Value = 1;
        players[1].GetComponent<PlayerStatus>().playerID.Value = 2;
    }

    // 通信でつながったプレイヤーがプレイシーンに来るので、それをとらえて位置を決める
    private void SearchPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
}