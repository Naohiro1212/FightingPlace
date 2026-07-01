using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float startCountDown = 3.0f;
    public TextMeshProUGUI timerText;

    // Playerの制御のために
    public PlayerStatus[] playerStatus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 開始時はプレイヤーは全員動けないようにする
        for (int i = 0; i < playerStatus.Length; i++)
        {
            playerStatus[i].canMove = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
