using UnityEngine;
using System.Collections;
using UnityEditor;
public class PCHandler : MonoBehaviour, IEscapeable
{
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject screenObj;
    [SerializeField] private GameObject screenOffObj;

    private bool isZoomed = false;

    PlayerInputManager cachedPlayerInput;

    void Start()
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; 
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = false;

        TurnOn();
    }

    public void ZoomToScreen(PlayerInteractionManager interactionManager)
    {
        if (isZoomed) return;

        if (interactionManager.TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.EscapeStack.Push(this);

            isZoomed = true;
            cachedPlayerInput = inputManager;
            ActivateUIScreen(inputManager);
        }
    }

    private void ActivateUIScreen(PlayerInputManager manager)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1; 
        canvasGroup.interactable = true; 
        canvasGroup.blocksRaycasts = true;

        manager.ToggleUIInput(true);
        manager.CameraManager.SetPanTiltEnabled(false);
        CursorUtils.UnlockAndShowCursor();
    }

    public void EscapePC()
    {
        if (cachedPlayerInput == null) return;

        isZoomed = false;

        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        cachedPlayerInput.ToggleUIInput(false);
        cachedPlayerInput.CameraManager.SetPanTiltEnabled(true);

        cachedPlayerInput = null;

        CursorUtils.LockAndHideCusor();
    }

    public void TurnOn()
    {
        screenOffObj.SetActive(false);
        screenObj.SetActive(true);
    }

    public void TurnOff()
    {
        screenOffObj.SetActive(true);
        screenObj.SetActive(false);
    }

    public void Escape(PlayerInputManager manager)
    {
        EscapePC();
    }
}