using UnityEngine;

public class BasicKenneyCharacterAnimationComponent : MonoBehaviour
{
    [SerializeField] private float _movementThreshold = 0.1f;

    private static readonly int IsMovingAnimationParameterHash = Animator.StringToHash("IsMoving");

    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Awake()
    {
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _animator);
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
}
