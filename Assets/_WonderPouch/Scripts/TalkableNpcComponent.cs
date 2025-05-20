using UnityEngine;

public class TalkableNpcComponent : MonoBehaviour
{
    [SerializeField] private string[] _dialogueLines;

    private DialogueSystem _dialogueSystem;

    public void Setup(DialogueSystem dialogueSystem)
    {
        _dialogueSystem = dialogueSystem;
    }

    public DialogueState Talk()
    {
        if (!_dialogueSystem)
        {
            return DialogueState.Invalid;
        }

        return _dialogueSystem.StartOrAdvanceDialogue(_dialogueLines);
    }
}
