using UnityEngine;
using System.Collections;
public class PCHandler : MonoBehaviour
{
    [SerializeField] private Animator camAnimator;
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject screenObj;
    [SerializeField] private GameObject screenOffObj;

    private bool isZoomed = false;

    public void ZoomToScreen()
    {
        if (isZoomed) return;

        isZoomed = true;
        camAnimator.SetTrigger("doZoom"); 
        StartCoroutine(ActivateUIScreen());
    }

    private IEnumerator ActivateUIScreen()
    {
        yield return new WaitForSeconds(1.0f);
        screen.SetActive(true);
    }

    public void ResetCamera()
    {
        isZoomed = false;
        camAnimator.SetTrigger("undoZoom"); 
        screen.SetActive(false);
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