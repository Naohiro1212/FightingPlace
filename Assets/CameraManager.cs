using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera[] virtualCameras;

    private void Start()
    {
        StartCoroutine(BindCameraCoroutine());
    }

    private IEnumerator BindCameraCoroutine()
    {
        while (true)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < players.Length; i++)
            {
                NetworkObject networkObject = players[i].GetComponent<NetworkObject>();

                if (networkObject != null && networkObject.IsOwner)
                {
                    Transform target = players[i].transform;

                    for (int j = 0; j < virtualCameras.Length; j++)
                    {
                        virtualCameras[j].Follow = target;
                        virtualCameras[j].LookAt = target;
                    }

                    yield break;
                }
            }

            yield return null;
        }
    }
}