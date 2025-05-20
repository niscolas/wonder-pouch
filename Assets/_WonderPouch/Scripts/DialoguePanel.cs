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

    public void Setup(DialogueSystem dialogueSystem)
    {
        if (!dialogueSystem)
        {
            return;
        }

        _dialogueSystem = dialogueSystem;
        _dialogueSystem.DialogueStarted += OnDialogueStarted;
        _dialogueSystem.DialogueAdvanced += OnDialogueStarted;
        _dialogueSystem.DialogueEnded += OnDialogueEnded;
    }

    private void OnDestroy()
    {
        if (_dialogueSystem)
        {
            _dialogueSystem.DialogueStarted -= OnDialogueStarted;
            _dialogueSystem.DialogueAdvanced -= OnDialogueAdvanced;
            _dialogueSystem.DialogueEnded -= OnDialogueEnded;
        }
    }

    private void OnDialogueStarted(string dialogueLine)
    {
        _onDialogueStartedUnityEventPre?.Invoke();

        UpdateText(dialogueLine);
    }

    private void OnDialogueAdvanced(string dialogueLine)
    {
        UpdateText(dialogueLine);
    }

    private void OnDialogueEnded()
    {
        _onDialogueEndedUnityEventPost?.Invoke();
    }

    public void UpdateText(string dialogueLine)
    {
        _dialogueText.SetText(dialogueLine);
    }
}
