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
            Debug.Log(playerStatuses[i].playerID.Value);
        }

        if (playerStatuses.Length != 2)
        {
            Debug.Log("正しいプレイヤーの数設定されていません");
        }

        // NetworkVariableの変更はサーバーのみ
        if (Unity.Netcode.NetworkManager.Singleton != null &&
            Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < playerStatuses.Length; i++)
            {
                playerStatuses[i].canMove.Value = false;
                playerStatuses[i].canShoot.Value = false;
            }
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

        if (Unity.Netcode.NetworkManager.Singleton != null &&
            Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < playerStatuses.Length; i++)
            {
                playerStatuses[i].canMove.Value = true;
                playerStatuses[i].canShoot.Value = true;
            }
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