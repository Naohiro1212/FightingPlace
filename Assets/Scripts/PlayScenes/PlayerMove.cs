using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 target;
    private Animator animator;
    private PlayerStatus playerStatus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.Log("rbが入っていません");

        playerStatus = GetComponent<PlayerStatus>();
        if (playerStatus == null) Debug.Log("playerStatusが入っていません");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.Log("animatorが入っていません");
    }

    public override void OnNetworkSpawn()
    {
        target = transform.position + transform.forward;
    }

    private void Update()
    {
        if (IsOwner)
        {
            UpdateOwnerInput();
            UpdateOwnerLookTarget();
            UpdateAnimator();
        }
    }

    private void FixedUpdate()
    {
        if (!IsSpawned) return;
        if (!IsOwner) return;
        if (playerStatus == null || !playerStatus.canMove) return;

        Move(moveInput);
        Rotate(target - transform.position);
    }

    private void UpdateOwnerInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null && playerStatus != null && playerStatus.canMove)
        {
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
        }

        input = input.normalized;
        moveInput = new Vector3(input.x, 0f, input.y);
    }

    private void UpdateOwnerLookTarget()
    {
        if (Mouse.current == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            target = ray.GetPoint(distance);
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        Vector3 localMove = transform.InverseTransformDirection(moveInput);

        animator.SetFloat("Horizontal", localMove.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", localMove.z, 0.1f, Time.deltaTime);
        animator.SetFloat("Speed", moveInput.magnitude, 0.1f, Time.deltaTime);
    }

    private void Move(Vector3 inputDirection)
    {
        Vector3 nextPosition = rb.position + inputDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void Rotate(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(rotation);
    }
}