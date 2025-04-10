using UnityEngine;
using System.Collections;
using UnityEditor;
using Unity.Cinemachine;
public class PCHandler : MonoBehaviour, IEscapeable
{
    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] CinemachineCamera pcZoomCam;

    private bool isZoomed = false;

    PlayerInputManager cachedPlayerInput;

    void Start()
    {
        SetCanvasGroupEnabled(false);
    }

    public void ZoomToScreen(PlayerInteractionManager interactionManager)
    {
        if (isZoomed) return;

        if (interactionManager.TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.EscapeStack.Push(this);

            pcZoomCam.enabled = true;
            isZoomed = true;
            cachedPlayerInput = inputManager;
            ActivateUIScreen(inputManager);
        }
    }

    private void ActivateUIScreen(PlayerInputManager manager)
    {
        SetCanvasGroupEnabled(true);

        manager.ToggleUIInput(true);
        manager.GetComponent<PlayerUI_Manager>().PlayerUI.SetPlayerUIEnabled(false);

        manager.CameraManager.SetPanTiltEnabled(false);
        CursorUtils.UnlockAndShowCursor();
    }

    public void EscapePC()
    {
        if (cachedPlayerInput == null) return;

        isZoomed = false;

        cachedPlayerInput.GetComponent<PlayerUI_Manager>().PlayerUI.SetPlayerUIEnabled(true);
        SetCanvasGroupEnabled(false);
        pcZoomCam.enabled = false;

        cachedPlayerInput.ToggleUIInput(false);
        cachedPlayerInput.CameraManager.SetPanTiltEnabled(true);

        cachedPlayerInput = null;

        CursorUtils.LockAndHideCusor();
    }

    public void SetCanvasGroupEnabled(bool enabled)
    {
        mainCanvasGroup.alpha = enabled ? 1 : 0;
        mainCanvasGroup.interactable = enabled;
        mainCanvasGroup.blocksRaycasts = enabled;
    }

    public void Escape(PlayerInputManager manager)
    {
        EscapePC();
    }
}