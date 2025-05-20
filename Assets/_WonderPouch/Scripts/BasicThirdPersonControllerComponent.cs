using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class BasicThirdPersonControllerComponent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Camera")]
    [SerializeField] private float _lookSensitivity = 1f;
    [SerializeField] private float _minVerticalAngle = -30f;
    [SerializeField] private float _maxVerticalAngle = 70f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInputNormalized;
    private Vector2 _lookInput;
    private bool _isGrounded;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private Transform _cameraTransform;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;

        _cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        CheckIsGrounded();
        MoveAndRotate();
    }

    public void HandleMoveInput(InputAction.CallbackContext context)
    {
        _moveInputNormalized = context.ReadValue<Vector2>().normalized;
    }

    public void HandleLookInput(InputAction.CallbackContext context)
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

    private void MoveAndRotate()
    {
        if (_moveInputNormalized == Vector2.zero)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        float targetRotationAngles = 0;
        targetRotationAngles =
            Mathf.Atan2(_moveInputNormalized.x, _moveInputNormalized.y) *
            Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;

        float currentYRotation = transform.rotation.eulerAngles.y;
        float smoothedRotationAngle = Mathf.LerpAngle(currentYRotation, targetRotationAngles, Time.deltaTime * _rotationSpeed);

        transform.rotation = Quaternion.Euler(0f, smoothedRotationAngle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, smoothedRotationAngle, 0f) * Vector3.forward;
        _rigidbody.linearVelocity = moveDirection.normalized * _moveSpeed;
    }
}
