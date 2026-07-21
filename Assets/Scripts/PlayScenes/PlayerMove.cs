using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 target;
    private Animator animator;
    private PlayerStatus playerStatus;
    private Vector3 input;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.Log("rbが入っていません");

        playerStatus = GetComponent<PlayerStatus>();
        if (playerStatus == null) Debug.Log("playerStatusが入っていません");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.Log("animatorが入っていません");

        var players = FindObjectsByType<PlayerStatus>();
        if (players.Length <= 1)
        {
            playerStatus.playerID = 1;
        }
        else
        {
            playerStatus.playerID = 2;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            Vector2 input = Vector2.zero;

            if (Keyboard.current != null && playerStatus.canMove != false)
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

            // 正規化して斜め移動が速くなりすぎないようにする
            input = input.normalized;

            // Vector2 → Vector3
            moveInput = new Vector3(input.x, 0f, input.y);

            // Animator パラメータ更新
            if (animator != null)
            {
                //Vector3 localMove = transform.InverseTransformDirection(moveInput);
                //animator.SetFloat("Horizontal", localMove.x);
                //animator.SetFloat("Vertical", localMove.z);
                //animator.SetFloat("Speed", moveInput.magnitude);

                //Debug.Log("Horizontal: " + localMove.x + ", Vertical: " + localMove.z + ", Speed: " + moveInput.magnitude);
            }

            // マウス位置から向き先を取得
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
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (playerStatus == null || !playerStatus.canMove) return;

        Vector3 localMove = transform.InverseTransformDirection(moveInput);
        animator.SetFloat("Horizontal", localMove.x);
        animator.SetFloat("Vertical", localMove.z);
        animator.SetFloat("Speed", moveInput.magnitude);

        // ローカルで即座に動かす(予測)
        Move(moveInput);
        Rotate(target - transform.position);

        // サーバーにも同じ入力を送る
        MoveServerRpc(moveInput.x, moveInput.z);
    }

    [ServerRpc]
    private void MoveServerRpc(float x, float z)
    {
        input = new Vector3(x, 0, z);

        Move(input);
        Rotate(target - transform.position);

        animator.SetFloat("Speed", input.magnitude);
        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.z);
    }

    private void Move(Vector3 input)
    {
        Vector3 nextPosition = rb.position + input * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(rotation);
    }
}