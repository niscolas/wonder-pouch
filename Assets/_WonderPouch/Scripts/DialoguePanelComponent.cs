using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DialoguePanelComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private Ease _fadeInEase = Ease.InSine;
    [SerializeField] private Ease _fadeOutEase = Ease.OutSine;

    [Header("References")]
    [SerializeField] private TMP_Text _npcNameText;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private CanvasGroup _visualRoot;

    [Header("Events")]
    [SerializeField] private UnityEvent _onDialogueStartedUnityEventPre;
    [SerializeField] private UnityEvent _onDialogueEndedUnityEventPost;

    private DialogueSystemComponent _dialogueSystem;

    public void Setup(DialogueSystemComponent dialogueSystem)
    {
        if (!dialogueSystem)
        {
            return;
        }

        _dialogueSystem = dialogueSystem;
        _dialogueSystem.DialogueStarted += OnDialogueStarted;
        _dialogueSystem.DialogueAdvanced += OnDialogueAdvanced;
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

    private void OnDialogueStarted(string npcName, string dialogueLine)
    {
        _onDialogueStartedUnityEventPre?.Invoke();

        _visualRoot.gameObject.SetActive(true);
        _visualRoot
            .DOFade(1, _fadeDuration)
            .From(0)
            .SetEase(_fadeInEase);

        _npcNameText.SetText(npcName);
        UpdateText(dialogueLine);
    }

    private void OnDialogueAdvanced(string dialogueLine)
    {
        UpdateText(dialogueLine);
    }

    private void OnDialogueEnded()
    {
        _visualRoot
            .DOFade(0, _fadeDuration)
            .SetEase(_fadeOutEase)
            .OnComplete(() =>
            {
                _visualRoot.gameObject.SetActive(false);
            });

        _onDialogueEndedUnityEventPost?.Invoke();
    }

    public void UpdateText(string dialogueLine)
    {
        _dialogueText.SetText(dialogueLine);
    }
}
