using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerStatus[] playerStatuses;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject fadeObject;

    [SerializeField] private string battleSceneName = "GameOver";

    public NetworkVariable<int> WinnerId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // NGO環境では、NetworkObjectが付いているオブジェクトにDontDestroyOnLoadを直接使うのは非推奨。
        // 下の OnNetworkSpawn の DestroyWithScene = false に管理を任せます。
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkObject != null)
        {
            NetworkObject.DestroyWithScene = false; // シーン遷移後も維持する
        }
    }

    private void Start()
    {
        // プレイヤーの検索・管理はサーバー側だけで十分
        if (IsServer)
        {
            StartCoroutine(FindPlayersCoroutine());
        }

        //WinnerIdを保存するので、DontDestroyOnLoadにする
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator FindPlayersCoroutine()
    {
        while (true)
        {
            // Unity 6向けにソートモードを明記（エラー防止）
            playerStatuses = FindObjectsByType<PlayerStatus>();
            if (playerStatuses != null && playerStatuses.Length >= 2)
            {
                break;
            }
            yield return null;
        }
    }

    public void OnPlayerDown(int downedPlayerId)
    {
        if (!IsServer) return;

        int winner = (downedPlayerId == 1) ? 2 : 1;
        WinnerId.Value = winner;

        try
        {
            ShowResultClientRpc(winner);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameManager] ShowResultClientRpc failed: {e}");
        }

        if (playerStatuses != null)
        {
            for (int i = 0; i < playerStatuses.Length; i++)
            {
                playerStatuses[i].canMove.Value = false;
                playerStatuses[i].canShoot.Value = false;
            }
        }

        StartCoroutine(FinishAfterDelay());
    }

    [ClientRpc]
    private void ShowResultClientRpc(int winner)
    {
        if (resultText != null)
        {
            resultText.text = "Player " + winner + " wins!";
            resultText.fontSize = 96;
        }
    }

    private IEnumerator FinishAfterDelay()
    {
        // 2.5秒間テキストを見せる
        yield return new WaitForSeconds(2.5f);

        // 全クライアントでフェードを開始
        StartFadeClientRpc();

        // フェード完了（1.0秒）を待つ
        yield return new WaitForSeconds(1.0f);

        if (IsServer)
        {
            LoadNextScene();
        }
    }

    [ClientRpc]
    private void StartFadeClientRpc()
    {
        if (fadeObject != null)
        {
            Fade fade = fadeObject.GetComponent<Fade>();
            if (fade != null)
            {
                fade.FadeIn(1f, null);
            }
        }
    }

    private void LoadNextScene()
    {
        //if (!IsServer) return;

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(battleSceneName, LoadSceneMode.Single);
        }
    }
}