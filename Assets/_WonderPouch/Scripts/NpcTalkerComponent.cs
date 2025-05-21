using UnityEngine;
using UnityEngine.Events;

public class NpcTalkerComponent : MonoBehaviour, InteractionHandler
{
    [Header("Events")]
    [SerializeField] private UnityEvent _onDialogueStartedUnityEvent;
    [SerializeField] private UnityEvent _onDialogueAdvancedUnityEvent;
    [SerializeField] private UnityEvent _onDialogueEndedUnityEvent;

    private TalkableNpcComponent _nearestTalkableNpc;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out TalkableNpcComponent talkableNpc))
        {
            return;
        }

        _nearestTalkableNpc = talkableNpc;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other || !_nearestTalkableNpc || other.gameObject != _nearestTalkableNpc.gameObject)
        {
            return;
        }

        _nearestTalkableNpc = null;
    }

    public bool TryHandleFirstAvailable()
    {
        if (!_nearestTalkableNpc)
        {
            return false;
        }

        DialogueState dialogueState = _nearestTalkableNpc.Talk();
        switch (dialogueState)
        {
            case DialogueState.Started:
                _onDialogueStartedUnityEvent?.Invoke();
                break;

            case DialogueState.Advanced:
                _onDialogueAdvancedUnityEvent?.Invoke();
                return true;

            case DialogueState.Ended:
                _onDialogueEndedUnityEvent?.Invoke();
                break;

            case DialogueState.Invalid:
            default:
                break;
        }

        return true;
    }
}
