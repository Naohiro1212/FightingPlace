using UnityEngine;
using TMPro;

// リザルト画面で表示するためのスクリプト
public class ResultPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;

    void Start()
    {
        // GameManager は DontDestroyOnLoad + DestroyWithScene=false で
        // バトルシーンからこのシーンまで生き残っているので、
        // WinnerId.Value は全クライアントで同じ値になっている
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager.Instance がありません（シーンをまたいで残っていない可能性）");
            return;
        }

        int winner = GameManager.Instance.WinnerId.Value;

        // 勝者に応じてテキストを書き換える
        if (winner == 1)
        {
            resultText.text = "Player 1 win!";
        }
        else if (winner == 2)
        {
            resultText.text = "Player 2 win!";
        }
    }
}