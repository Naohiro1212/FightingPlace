using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    private int minPoint = 2;

    private void Start()
    {
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
        player1.position = spawnPoints[index1].position + Vector3.up * 1.0f;
        player2.position = spawnPoints[index2].position + Vector3.up * 1.0f;
    }
}
