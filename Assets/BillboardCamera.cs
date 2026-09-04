using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera targetCamera;

    private void Start()
    {
        targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
            {
                return;
            }
        }

        // ƒJƒƒ‰‚Æ“¯‚¶Œü‚«‚É‚·‚é
        transform.rotation = targetCamera.transform.rotation;
    }
}