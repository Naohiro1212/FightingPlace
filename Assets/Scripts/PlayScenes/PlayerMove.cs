using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float stickDeadZone = 0.1f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 lookDirection;

    private Animator animator;
    private PlayerStatus playerStatus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.Log("rbが入っていません");
        }

        playerStatus = GetComponent<PlayerStatus>();

        if (playerStatus == null)
        {
            Debug.Log("playerStatusが入っていません");
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.Log("animatorが入っていません");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // 最初は現在向いている方向
        lookDirection = transform.forward;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        UpdateControllerInput();
        UpdateControllerLookDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!IsSpawned)
        {
            return;
        }

        if (!IsOwner)
        {
            return;
        }

        if (playerStatus == null ||
            !playerStatus.canMove.Value)
        {
            return;
        }

        Move(moveInput);

        Rotate(lookDirection);
    }

    // =========================================
    // 左スティック移動
    // =========================================
    private void UpdateControllerInput()
    {
        if (playerStatus == null ||
            !playerStatus.canMove.Value)
        {
            moveInput = Vector3.zero;
            return;
        }

        if (Gamepad.current == null)
        {
            moveInput = Vector3.zero;
            return;
        }

        Vector2 input =
            Gamepad.current.leftStick.ReadValue();

        // 左スティックを触っていなければ十字キー
        if (input.sqrMagnitude <
            stickDeadZone * stickDeadZone)
        {
            input =
                Gamepad.current.dpad.ReadValue();
        }

        input =
            Vector2.ClampMagnitude(
                input,
                1f
            );

        moveInput = new Vector3(
            input.x,
            0f,
            input.y
        );
    }

    // =========================================
    // 右スティックで向きを変更
    // =========================================
    private void UpdateControllerLookDirection()
    {
        if (Gamepad.current == null)
        {
            return;
        }

        Vector2 lookInput =
            Gamepad.current.rightStick.ReadValue();

        // 右スティックを倒していない
        if (lookInput.sqrMagnitude <
            stickDeadZone * stickDeadZone)
        {
            // lookDirectionを変更しない
            // → 最後の角度をそのまま維持
            return;
        }

        lookDirection = new Vector3(
            lookInput.x,
            0f,
            lookInput.y
        ).normalized;
    }

    // =========================================
    // Animator
    // =========================================
    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        Vector3 localMove =
            transform.InverseTransformDirection(
                moveInput
            );

        animator.SetFloat(
            "Horizontal",
            localMove.x,
            0.1f,
            Time.deltaTime
        );

        animator.SetFloat(
            "Vertical",
            localMove.z,
            0.1f,
            Time.deltaTime
        );

        animator.SetFloat(
            "Speed",
            moveInput.magnitude,
            0.1f,
            Time.deltaTime
        );
    }

    // =========================================
    // 移動
    // =========================================
    private void Move(
        Vector3 inputDirection)
    {
        Vector3 nextPosition =
            rb.position +
            inputDirection *
            moveSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    // =========================================
    // 回転
    // =========================================
    private void Rotate(
        Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion rotation =
            Quaternion.LookRotation(direction);

        rb.MoveRotation(rotation);
    }
}