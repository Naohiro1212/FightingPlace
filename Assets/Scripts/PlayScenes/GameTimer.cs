using TMPro;
using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    private float startCountDown = 3.0f;
    public TextMeshProUGUI timerText;

    // Playerの制御のために
    public PlayerStatus[] playerStatuses;

    private bool started = false;

    private void Start()
    {
        StartCoroutine(FindPlayersCoroutine());

        // 開始時はプレイヤーは全員動けないようにする
        for (int i = 0; i < playerStatuses.Length; i++)
        {
            playerStatuses[i].canMove = false;
        }
    }

    private IEnumerator FindPlayersCoroutine()
    {
        while (true)
        {
            playerStatuses = FindObjectsByType<PlayerStatus>();
            if (playerStatuses != null && playerStatuses.Length >= 2)
            {
                break;
            }

            yield return null;
        }

        for (int i = 0; i < playerStatuses.Length; i++)
        {
            Debug.Log(playerStatuses[i].playerID);
        }

        if (playerStatuses.Length != 2)
        {
            Debug.Log("正しいプレイヤーの数設定されていません");
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

        for(int i = 0;i < playerStatuses.Length; i++)
        {
            playerStatuses[i].canMove = true;
            playerStatuses[i].canShoot = true;
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
