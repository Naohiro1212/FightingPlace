using UnityEngine;

public class DisconnectManager : MonoBehaviour
{
    private NetworkManagerController networkController;

    private void Start()
    {
        networkController = NetworkManagerController.Instance;

        if (networkController == null)
        {
            Debug.LogError("NetworkManagerControllerが見つかりません");
        }
    }

    public void Disconnect()
    {
        if (networkController == null)
        {
            return;
        }

        networkController.Disconnect();
    }
}