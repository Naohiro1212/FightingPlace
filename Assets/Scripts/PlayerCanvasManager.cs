using UnityEngine;

public class PlayerCanvasManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player1Canvas;

    [SerializeField]
    private GameObject player2Canvas;

    private void Awake()
    {
        player1Canvas.SetActive(false);
        player2Canvas.SetActive(false);
    }

    public void SetPlayerCanvas(int playerID)
    {
        Debug.Log(
            $"[PlayerCanvasManager] SetPlayerCanvas ID={playerID}"
        );

        if (playerID == 1)
        {
            player1Canvas.SetActive(true);
            player2Canvas.SetActive(false);

            Debug.Log("Player1Canvas ON");
        }
        else if (playerID == 2)
        {
            player1Canvas.SetActive(false);
            player2Canvas.SetActive(true);

            Debug.Log("Player2Canvas ON");
        }
        else
        {
            Debug.LogWarning(
                $"PlayerIDがおかしいです: {playerID}"
            );
        }
    }
}