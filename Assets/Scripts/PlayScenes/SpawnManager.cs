using System.Runtime.InteropServices.WindowsRuntime;
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
    }

    // 通信でつながったプレイヤーがプレイシーンに来るので、それをとらえて位置を決める
    private void SearchPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
}
