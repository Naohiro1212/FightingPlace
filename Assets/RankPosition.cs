using UnityEngine;
using UnityEngine.Rendering;

public class RankPosition : MonoBehaviour
{
    [SerializeField] GameObject[] rankPositions;
    [SerializeField] GameObject[] players;
    void Start()
    {
        if (GameManager.winnerId == 1)
        {
            Debug.Log("勝者は1");
            players[0].transform.position = rankPositions[0].transform.position + rankPositions[0].transform.up * 3.0f;
            players[1].transform.position = rankPositions[1].transform.position + rankPositions[1].transform.up * 3.0f;
        }
        else
        {
            Debug.Log("勝者は2");
            players[0].transform.position = rankPositions[1].transform.position + rankPositions[1].transform.up * 3.0f;
            players[1].transform.position = rankPositions[0].transform.position + rankPositions[0].transform.up * 3.0f;
        }
    }
}
