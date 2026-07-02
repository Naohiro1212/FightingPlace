using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    // 入力とマウスの方向に動く
    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 target;

    // プレイヤーのステータスを取得するための変数
    PlayerStatus playerStatus;

    private void Start()
    {
        // Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.Log("rbが入っていません");

        // プレイヤーのステータス
        playerStatus = GetComponent<PlayerStatus>();
        if (playerStatus == null) Debug.Log("playerStatusが入っていません");

    }

    // 入力受付のみ
    private void Update()
    { 
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (playerStatus.playerID == 1)
            {
                if (Keyboard.current.aKey.isPressed) input.x -= 1f;
                if (Keyboard.current.dKey.isPressed) input.x += 1f;
                if (Keyboard.current.sKey.isPressed) input.y -= 1f;
                if (Keyboard.current.wKey.isPressed) input.y += 1f;
            }
            else if (playerStatus.playerID == 2)
            {
                if (Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
                if (Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
                if (Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
                if (Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            }
        }

        moveInput = new Vector3(input.x, 0f, input.y).normalized;

        // マウスの位置を取得してターゲットを更新
        if (Mouse.current != null && Camera.main != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(ray, out float distance))
            {
                target = ray.GetPoint(distance);
            }
        }
    }

    // 実際に動く
    private void FixedUpdate()
    {
        Vector3 nextPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        
        if (playerStatus.canMove == true)
        {
            rb.MovePosition(nextPosition);
        }

        // canMoveがTrueの時のプレイヤーの向きをマウス方向に向ける
        if(playerStatus.canMove == true)
        {
            Vector3 lookDirection = target - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);
                rb.MoveRotation(rotation);
            }
        }
    }
}