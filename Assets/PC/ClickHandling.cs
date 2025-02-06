using UnityEngine;

public class ClickHandling : MonoBehaviour
{
    [SerializeField] private Camera cam; 
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Clickable")) 
                {
                    hit.collider.GetComponent<ClickableObjs>()?.OnClick();
                }
            }
        }
    }
}
