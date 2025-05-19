using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BasicThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _lookSensitivity = 1f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CheckIsGrounded();
        HandleRotation();
        HandleCameraLook();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void CheckIsGrounded()
    {
        _isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            _groundCheckDistance,
            _groundLayer
        );
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        moveDirection = _cameraTarget.TransformDirection(moveDirection);
        moveDirection.y = 0f;

        Vector3 targetVelocity = moveDirection * _moveSpeed;
        _rigidbody.linearVelocity = new Vector3(
            targetVelocity.x,
            _rigidbody.linearVelocity.y,
            targetVelocity.z
        );
    }

    private void HandleRotation()
    {
        if (_moveInput.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg + _cameraTarget.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleCameraLook()
    {
        if (_lookInput.magnitude > 0.1f)
        {
            _cameraTarget.Rotate(Vector3.up, _lookInput.x * _lookSensitivity);
        }
    }
}
