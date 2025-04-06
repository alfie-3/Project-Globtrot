using UnityEngine;
using System.Collections;
public class PCHandler : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject screenObj;
    [SerializeField] private GameObject screenOffObj;

    private bool isZoomed = false;

    void Start()
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; 
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = false;

        TurnOn();
    }

    public void ZoomToScreen()
    {
        if (isZoomed) return;

        isZoomed = true;
        ActivateUIScreen();
    }

    private void ActivateUIScreen()
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1; 
        canvasGroup.interactable = true; 
        canvasGroup.blocksRaycasts = true; 

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void ResetCamera()
    {
        isZoomed = false;
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; 
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = false; 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetCamera();
        }
    }
}