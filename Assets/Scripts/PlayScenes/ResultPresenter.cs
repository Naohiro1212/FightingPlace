using UnityEngine;
using TMPro;

// リザルト画面で表示するためのスクリプト
public class ResultPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    void Start()
    {
        int winner = GameManager.winnerId;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
