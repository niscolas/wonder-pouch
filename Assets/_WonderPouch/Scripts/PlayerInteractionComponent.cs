using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionComponent : MonoBehaviour
{
    private InteractionHandler[] _interactionHandlers;

    private void Awake()
    {
        _interactionHandlers = GetComponentsInChildren<InteractionHandler>();
    }

    public void HandleInteractionInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        TryInteract();
    }

    public void TryInteract()
    {
        foreach (var interactionHandler in _interactionHandlers)
        {
            if (interactionHandler.TryHandleFirstAvailable())
            {
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out InteractionIndicatorComponent interactionIndicator))
        {
            return;
        }

        interactionIndicator.ShowInteractionIndication();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out InteractionIndicatorComponent interactionIndicator))
        {
            return;
        }

        interactionIndicator.HideInteractionIndication();
    }
}
