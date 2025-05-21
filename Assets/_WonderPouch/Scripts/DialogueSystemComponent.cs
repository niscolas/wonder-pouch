using System;
using UnityEngine;

public class DialogueSystemComponent : MonoBehaviour
{
    [SerializeField] private DialoguePanelComponent _dialoguePanel;

    public event Action<string, string> DialogueStarted;
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

    public DialogueState StartOrAdvanceDialogue(string npcName, string[] dialogueLines)
    {
        if (dialogueLines.IsNullOrEmpty())
        {
            return DialogueState.Invalid;
        }

        _dialogueLines = dialogueLines;

        if (_currentDialogueLineIndex == 0)
        {
            DialogueStarted?.Invoke(npcName, _dialogueLines[_currentDialogueLineIndex]);
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
