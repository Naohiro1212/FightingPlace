using System.Runtime.CompilerServices;
using UnityEngine;

// <summary>
// ターゲットを探すためのクラス
// </summary>
public class PlayerSearch : MonoBehaviour
{
    [SerializeField] private float angle = 45f;
    [SerializeField] private Camera targetCamera;

    private Transform currentTarget;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("target")) // 視界の範囲内の当たり判定
        {
            // 視界の角度内におさまっているか
            Vector3 posDelta = other.transform.position - this.transform.position;
            float target_angle = Vector3.Angle(this.transform.forward, posDelta);

            bool found = false;

            if (target_angle < angle)
            {
                if (Physics.Raycast(this.transform.position, posDelta, out RaycastHit hit))//Rayを使用してtargetに当たっているか判別
                {
                    if (hit.collider == other)
                    {
                        Debug.Log("range of view");
                        found = true;
                    }
                }
            }

            if (found)
            {
                currentTarget = other.transform;
            }
            else
            {
                if (currentTarget == other.transform)
                {
                    currentTarget = null;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("target")) return;

        if(currentTarget == other.transform)
        {
            currentTarget = null;
        }
    }
}
