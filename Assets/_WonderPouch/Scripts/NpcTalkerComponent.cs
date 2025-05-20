using UnityEngine;

public class NpcTalkerComponent : MonoBehaviour, InteractionHandler
{
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
        if (other == null || other.gameObject != _nearestTalkableNpc.gameObject)
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

        _nearestTalkableNpc.Talk();
        return true;
    }
}
