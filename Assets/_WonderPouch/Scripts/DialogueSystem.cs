using System;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private DialoguePanel _dialoguePanel;

    public event Action<string> DialogueStarted;
    public event Action<string> DialogueAdvanced;
    public event Action DialogueEnded;

    private string[] _dialogueLines;
    private int _currentDialogueLineIndex;

    private void Awake()
    {
        if (_dialoguePanel)
        {
            _dialoguePanel.Setup(this);
        }
    }

    public DialogueState StartOrAdvanceDialogue(string[] dialogueLines)
    {
        if (dialogueLines.IsNullOrEmpty())
        {
            return DialogueState.Invalid;
        }

        _dialogueLines = dialogueLines;

        if (_currentDialogueLineIndex == 0)
        {
            DialogueStarted?.Invoke(_dialogueLines[_currentDialogueLineIndex]);
            _currentDialogueLineIndex++;
            return DialogueState.Started;
        }
        else if (_currentDialogueLineIndex < _dialogueLines.Length)
        {
            DialogueAdvanced?.Invoke(_dialogueLines[_currentDialogueLineIndex]);
            _currentDialogueLineIndex++;
            return DialogueState.Advanced;
        }
        else
        {
            DialogueEnded?.Invoke();
            _currentDialogueLineIndex = 0;
            return DialogueState.Ended;
        }
    }
}
