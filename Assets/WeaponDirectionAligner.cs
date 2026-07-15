using UnityEngine;

// プレイヤーの向きに合わせて武器の向きを補正する
public class WeaponDirectionAligner : MonoBehaviour
{
    // グリップ位置の調整用オフセット
    private Vector3 gripOffset = Vector3.zero;

    void Update()
    {
        // プレイヤーの向きに合わせて武器の向きを補正する
        Vector3 playerForward = transform.root.forward;
        playerForward.y = 0f;

        // 手の位置も補正し、右手にトリガーがかかっている状態にする
        Vector3 playerHand = transform.parent.position;

        if (playerForward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = targetRotation;

            // 回転後のローカル軸に沿ってオフセットを加算し、グリップ位置を合わせる
            transform.position = playerHand + (targetRotation * gripOffset);
        }
    }
}
