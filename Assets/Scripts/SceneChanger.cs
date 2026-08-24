using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneName = "LobbyScene";

    public void LoadNextScene()
    {
        // 自分の端末のネットワークを終了
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // 自分だけタイトルへ戻る
        SceneManager.LoadScene(
            sceneName,
            LoadSceneMode.Single
        );
    }
}