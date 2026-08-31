using TMPro;
using UnityEngine;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI connectionStatusText;

    private void Start()
    {
        if (NetworkManagerController.Instance == null)
        {
            Debug.LogError(
                "NetworkManagerControllerがありません"
            );

            return;
        }

        NetworkManagerController.Instance
            .SetConnectionStatusText(connectionStatusText);
    }

    public void StartHost()
    {
        NetworkManagerController.Instance?.StartAsHost();
    }

    public void StartClient()
    {
        NetworkManagerController.Instance?.StartAsClient();
    }

    public void GameEnd()
    {
        NetworkManagerController.Instance?.GameEnd();
    }
}