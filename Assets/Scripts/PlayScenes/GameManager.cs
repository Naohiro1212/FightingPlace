using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // プレイヤーの生死を確認するための配列
    [SerializeField] private PlayerStatus[] playerStatuses;

    // プレイ画面で一時的に勝利者を表示する為のUIテキスト
    [SerializeField] private TextMeshProUGUI resultText;

    // シーン遷移用のFadeオブジェクト
    [SerializeField] private GameObject fadeObject;

    public static int winnerId { get; private set; } = 0; // 勝者のIDを保持する変数

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 既に存在するなら自分を破棄
            return;
        }
        Instance = this;

        // 正しいプレイヤーの数が設定されているか確認
        if (playerStatuses.Length != 2)
        {
            Debug.Log("正しいプレイヤーの数設定されていません");
        }

        // resultTextが設定されているか確認
        if (resultText == null)
        {
            Debug.Log("resultTextが設定されていません");
        }

        // FadeObjectが設定されているか確認
        if (fadeObject == null)
        {
            Debug.Log("fadeObjectが設定されていません");
        }
    }

    public void OnPlayerDown(int downedPlayerId)
    {
        // downedPlayerIdが1なら2が勝者、downedPlayerIdが2なら1が勝者
        winnerId = (downedPlayerId == 1) ? 2 : 1;
        ShowResult(winnerId);

        // 2.5秒後にシーン遷移演出を開始
        StartCoroutine(FinishAfterDelay());

        // プレイヤーをどちらも動けない状態にする
        for (int i = 0; i < playerStatuses.Length; i++)
        {
            playerStatuses[i].canMove = false;
            playerStatuses[i].canShoot = false;
        }
    }

    private void ShowResult(int winnerId)
    {
        resultText.text = "Player " + winnerId + " wins!";
        resultText.fontSize = 96;
    }

    private void CallScene()
    {
        // Fadeを取得し、FadeInする
        Fade fade = fadeObject.GetComponent<Fade>();
        fade.FadeIn(1f, () =>
        {
            SceneManager.LoadScene("GameOver");
        });
    }

    IEnumerator FinishAfterDelay()
    {
        // 2.5秒間待機する
        yield return new WaitForSeconds(2.5f);

        // シーン遷移開始
        CallScene();
    }
}