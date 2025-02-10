using UnityEngine;

public class BillboardUI : MonoBehaviour
{

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}