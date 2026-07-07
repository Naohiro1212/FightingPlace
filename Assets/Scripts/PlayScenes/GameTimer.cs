using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float startCountDown = 3.0f;
    public TextMeshProUGUI timerText;

    // Playerの制御のために
    public PlayerStatus[] playerStatus;

    private bool started = false;

    void Start()
    {
        // 開始時はプレイヤーは全員動けないようにする
        for (int i = 0; i < playerStatus.Length; i++)
        {
            playerStatus[i].canMove = false;
        }
    }
    private void Update()
    {
        if (started) return;

        startCountDown -= Time.deltaTime;

        if (startCountDown > 0)
        {
            timerText.text = Mathf.Ceil(startCountDown).ToString();
        }
        else
        {
            timerText.text = "START!";
            timerText.fontSize = 96;
            StartGame();
        }
    }

    private void StartGame()
    {
        started = true;

        for(int i = 0;i < playerStatus.Length; i++)
        {
            playerStatus[i].canMove = true;
            playerStatus[i].canShoot = true;
        }

        // 少し後に文字を消す
        Invoke(nameof(HideText), 1f);
    }

    private void HideText()
    {
        timerText.text = "";
    }

    public bool IsStarted()
    {
        return started;
    }
}
