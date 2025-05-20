using System;
using UnityEngine;

public class TalkableNpcComponent : MonoBehaviour
{
    [SerializeField] private string[] _dialogueLines;

    private DialogueSystem _dialogueSystem;

    public void Setup(DialogueSystem dialogueSystem)
    {
        _dialogueSystem = dialogueSystem;
    }

    public void Talk()
    {
        if (!_dialogueSystem)
        {
            return;
        }

        _dialogueSystem.StartDialogue(_dialogueLines);
    }
}
