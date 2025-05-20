using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Events")]
    [SerializeField] private UnityEvent _onDialogueStartedUnityEventPre;
    [SerializeField] private UnityEvent _onDialogueEndedUnityEventPost;

    private DialogueSystem _dialogueSystem;
    private string[] _dialogueLines;
    private int _currentLineIndex;

    public void Setup(DialogueSystem dialogueSystem)
    {
        if (!dialogueSystem)
        {
            return;
        }

        _dialogueSystem = dialogueSystem;
        _dialogueSystem.DialogueStarted += OnDialogueStarted;
    }

    private void OnDestroy()
    {
        if (_dialogueSystem)
        {
            _dialogueSystem.DialogueStarted -= OnDialogueStarted;
        }
    }

    private void OnDialogueStarted(string[] dialogueLines)
    {
        _onDialogueStartedUnityEventPre?.Invoke();

        _dialogueLines = dialogueLines;
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (_currentLineIndex >= _dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        _dialogueText.SetText(_dialogueLines[_currentLineIndex]);
        _currentLineIndex++;
    }

    private void EndDialogue()
    {
        _onDialogueEndedUnityEventPost?.Invoke();
    }
}
