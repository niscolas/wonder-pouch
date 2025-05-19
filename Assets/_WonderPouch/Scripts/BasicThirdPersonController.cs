using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class BasicThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private float _lookSensitivity = 1f;
    [SerializeField] private float _minVerticalAngle = -30f;
    [SerializeField] private float _maxVerticalAngle = 70f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInputNormalized;
    private Vector2 _lookInput;
    private bool _isGrounded;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CheckIsGrounded();
        MoveAndRotate();
        RotateCamera();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        _moveInputNormalized = context.ReadValue<Vector2>().normalized;
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

    private void MoveAndRotate()
    {
        if (_moveInputNormalized == Vector2.zero)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        Transform cameraTransform = _cinemachineCamera.transform;

        float targetRotationAngles = 0;
        targetRotationAngles =
            Mathf.Atan2(_moveInputNormalized.x, _moveInputNormalized.y) *
            Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

        transform.rotation = Quaternion.Euler(0f, targetRotationAngles, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetRotationAngles, 0f) * Vector3.forward;
        _rigidbody.linearVelocity = moveDirection.normalized * _moveSpeed;
        Debug.Log($"{_moveInputNormalized} .. {_rigidbody.linearVelocity}");

    }

    private void RotateCamera()
    {
        if (_lookInput.magnitude >= 0.1f)
        {
            _cinemachineTargetYaw += _lookInput.x * Time.deltaTime;
            _cinemachineTargetPitch += _lookInput.y * Time.deltaTime;
        }

        _cinemachineTargetYaw += _lookInput.x * _lookSensitivity;
        _cinemachineTargetPitch -= _lookInput.y * _lookSensitivity;
        _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, _minVerticalAngle, _maxVerticalAngle);

        Transform cameraTarget = _cinemachineCamera.Follow;
        cameraTarget.rotation = Quaternion.Euler(
            _cinemachineTargetPitch,
            _cinemachineTargetYaw,
            0f
        );
    }
}
