using UnityEngine;
using System.Collections;
public class PCHandler : MonoBehaviour
{
    [SerializeField] private Animator camAnimator;
    [SerializeField] private GameObject screen;

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
        yield return new WaitForSeconds(1.5f);
        screen.SetActive(true);
    }

    public void ResetCamera()
    {
        isZoomed = false;
        camAnimator.SetTrigger("undoZoom"); 
        screen.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetCamera();
        }
    }

}