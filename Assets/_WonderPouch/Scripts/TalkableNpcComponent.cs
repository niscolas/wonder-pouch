using UnityEngine;

public class TalkableNpcComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string _npcName;
    [SerializeField] private string[] _dialogueLines;

    private DialogueSystemComponent _dialogueSystem;

    public void Setup(DialogueSystemComponent dialogueSystem)
    {
        _dialogueSystem = dialogueSystem;
    }

    public DialogueState Talk()
    {
        if (!_dialogueSystem)
        {
            return DialogueState.Invalid;
        }

        return _dialogueSystem.StartOrAdvanceDialogue(_npcName, _dialogueLines);
    }
}
