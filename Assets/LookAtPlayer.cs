using DG.Tweening;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] float lookActivateDistance = 3f;
    [SerializeField] float lookTime = 0.5f;
    // Update is called once per frame

    Quaternion prevRot;
    Vector3 lookPos;
    Quaternion lookRot;

    void LateUpdate()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        
        if (Vector3.Distance(transform.position, cameraPos) <= lookActivateDistance)
        {
            
            //transform.LookAt(cameraPos, lookPos);
            lookPos = transform.up + transform.forward;
            lookRot = Quaternion.LookRotation((cameraPos - transform.position), transform.forward);

            transform.rotation = Quaternion.Slerp(prevRot, lookRot, lookTime * Time.deltaTime);

            prevRot = transform.rotation;
        }
        else
        {
            //transform.rotation = Quaternion.Slerp(lookRot, prevRot, lookTime * Time.deltaTime);
        }
    }
}
