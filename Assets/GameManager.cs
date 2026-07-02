using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // プレイヤーの生死を確認するための配列
    [SerializeField] private PlayerStatus[] playerStatuses;

    // プレイ画面で一時的に勝利者を表示する為のUIテキスト
    [SerializeField] private TextMeshProUGUI resultText;

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
    }

    public void OnPlayerDown(int downedPlayerId)
    {
        // downedPlayerIdが1なら2が勝者、downedPlayerIdが2なら1が勝者
        int winnerId = (downedPlayerId == 1) ? 2 : 1;
        ShowResult(winnerId);

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
}