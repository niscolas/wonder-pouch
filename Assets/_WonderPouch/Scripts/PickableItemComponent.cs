using UnityEngine;
using DG.Tweening;

public class PickableItemComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InventoryItem _itemData;

    [Header("Settings / Look & Feel")]
    [SerializeField] private bool _isTweeningEnabled;
    [SerializeField] private Vector3 _floatOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private float _floatDuration = 1f;
    [SerializeField] private Ease _floatEase = Ease.InOutSine;
    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private Ease _rotationEase = Ease.InOutSine;

    public InventoryItem ItemData { get => _itemData; }

    private void Awake()
    {
        if (_itemData.currentStack == 0)
        {
            _itemData.currentStack = 1;
        }
    }

    private void Start()
    {
        if (_isTweeningEnabled)
        {
            transform
                .DOMove(_floatOffset, _floatDuration)
                .SetEase(_floatEase)
                .SetRelative()
                .SetLoops(-1, LoopType.Yoyo);

            transform.DORotate(new Vector3(0, 360, 0), _rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(_rotationEase)
                     .SetLoops(-1, LoopType.Incremental);
        }
    }
}
