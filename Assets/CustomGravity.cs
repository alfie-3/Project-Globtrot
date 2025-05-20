using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [SerializeField] Vector3 force;
    [SerializeField] Rigidbody rb;
    private void OnEnable()
    {
        if (gameObject.GetComponent<Rigidbody>() == null) return;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {        
        rb.AddForce(force);
    }
}
