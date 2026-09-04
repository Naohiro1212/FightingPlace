using UnityEngine;

public class RankPosition : MonoBehaviour
{
    [SerializeField] private GameObject[] rankPositions;
    [SerializeField] private GameObject[] players;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager.Instance がありません（シーンをまたいで残っていない可能性）");
            return;
        }

        int winner = GameManager.Instance.WinnerId.Value;

        if (winner == 1)
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