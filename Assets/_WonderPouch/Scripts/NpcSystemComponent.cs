using UnityEngine;

public class NpcSystemComponent : MonoBehaviour
{
    [SerializeField] private DialogueSystemComponent _dialogueSystem;

    private void Awake()
    {
        TalkableNpcComponent[] talkableNPCs = Object.FindObjectsByType<TalkableNpcComponent>(FindObjectsSortMode.None);

        foreach (TalkableNpcComponent npc in talkableNPCs)
        {
            npc.Setup(_dialogueSystem);
        }
    }
}
