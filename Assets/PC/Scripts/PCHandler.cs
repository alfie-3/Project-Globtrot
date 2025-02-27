using UnityEngine;
using System.Collections;
public class PCHandler : MonoBehaviour
{
    // [SerializeField] private Animator camAnimator;
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
    }

    public void ZoomToScreen()
    {
        if (isZoomed) return;

        isZoomed = true;
        // camAnimator.SetTrigger("doZoom"); 
        StartCoroutine(ActivateUIScreen());
    }

    private IEnumerator ActivateUIScreen()
    {
        yield return new WaitForSeconds(1.0f);
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1; 
        canvasGroup.interactable = true; 
        canvasGroup.blocksRaycasts = true; 

    }

    public void ResetCamera()
    {
        isZoomed = false;
        // camAnimator.SetTrigger("undoZoom"); 
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; 
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = false; 

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