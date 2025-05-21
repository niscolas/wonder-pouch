using UnityEngine;

public class BasicKenneyCharacterAnimationComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _movementThreshold = 0.1f;
    [SerializeField] private bool _startSitting;

    private static readonly int IsMovingAnimationParameterHash = Animator.StringToHash("IsMoving");
    private static readonly int IsSittingAnimationParameterHash = Animator.StringToHash("IsSitting");
    private static readonly int IsTalkingAnimationParameterHash = Animator.StringToHash("IsTalking");
    private static readonly int InteractAnimationParameterHash = Animator.StringToHash("Interact");

    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _animator);

        if (_startSitting && _animator)
        {
            _animator.SetBool(IsSittingAnimationParameterHash, true);
        }
    }

    private void Update()
    {
        if (!_rigidbody || !_animator)
        {
            return;
        }

        Vector3 velocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        bool isMoving = velocity.magnitude > _movementThreshold;

        _animator.SetBool(IsMovingAnimationParameterHash, isMoving);
    }

    public void SetIsTalking(bool value)
    {
        if (_animator)
        {
            _animator.SetBool(IsTalkingAnimationParameterHash, value);
        }
    }

    public void TriggerInteract()
    {
        if (_animator)
        {
            _animator.SetTrigger(InteractAnimationParameterHash);
        }
    }
}
