using UnityEngine;

public class NpcSystem : MonoBehaviour
{
    [SerializeField] private DialogueSystem _dialogueSystem;

    private void Awake()
    {
        TalkableNpcComponent[] talkableNPCs = Object.FindObjectsByType<TalkableNpcComponent>(FindObjectsSortMode.None);

        foreach (TalkableNpcComponent npc in talkableNPCs)
        {
            npc.Setup(_dialogueSystem);
        }
    }
}
