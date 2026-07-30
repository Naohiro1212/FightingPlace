using Unity.Netcode;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera[] virtualCameras;

    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var playerObj in players)
        {
            NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();

            if (networkObject != null && networkObject.IsOwner)
            {
                // 自分自身のプレイヤーだけをカメラに追従させる
                Transform target = playerObj.transform;

                foreach (var vcam in virtualCameras)
                {
                    vcam.Follow = target;
                    vcam.LookAt = target;
                }

                break; // 自分の分が見つかったら終了
            }
        }
    }
}