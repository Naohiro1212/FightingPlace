using UnityEngine;

public class PlayerCanvasManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player1Canvas;

    [SerializeField]
    private GameObject player2Canvas;

    private void Start()
    {
        // 最初はいったん両方消す
        player1Canvas.SetActive(false);
        player2Canvas.SetActive(false);
    }

    public void SetPlayerCanvas(int playerID)
    {
        if (playerID == 1)
        {
            player1Canvas.SetActive(true);
            player2Canvas.SetActive(false);
        }
        else if (playerID == 2)
        {
            player1Canvas.SetActive(false);
            player2Canvas.SetActive(true);
        }
    }
}