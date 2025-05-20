using System;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private DialoguePanel _dialoguePanel;

    public event Action<string[]> DialogueStarted;

    private void Awake()
    {
        if (_dialoguePanel)
        {
            _dialoguePanel.Setup(this);
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        if (dialogueLines.IsNullOrEmpty())
        {
            return;
        }

        DialogueStarted?.Invoke(dialogueLines);
    }
}
