using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomPlayerInputManagerComponent : MonoBehaviour
{
    [SerializeField] private string _defaultActionMap = "Player";
    [SerializeField] private string _npcTalkActionMap = "NpcTalk";
    [SerializeField] private string _inventoryActionMap = "Inventory";

    private PlayerInput _playerInput;

    private void Start()
    {
        TryGetComponent(out _playerInput);

        StartCoroutine(LateStart());
    }

    // NOTE: 
    // This is a workaround for a bug in the version of Input System present in the project
    // the action maps can't be disabled on Start (possibly due to race conditions)
    // https://discussions.unity.com/t/input-system-i-cant-disable-action-maps-on-awake-start/1607233/2
    private IEnumerator LateStart()
    {
        yield return null;
        yield return null;

        SwitchToDefaultActionMap();
    }

    public void SwitchToDefaultActionMap()
    {
        SwitchToActionMap(_defaultActionMap);
    }

    public void SwitchToInventoryActionMap()
    {
        SwitchToActionMap(_inventoryActionMap);
    }

    public void SwitchToNpcTalkActionMap()
    {
        SwitchToActionMap(_npcTalkActionMap);
    }

    public void SwitchToActionMap(string actionMapName)
    {
        _playerInput.actions.Disable();
        _playerInput.SwitchCurrentActionMap(actionMapName);
    }
}
