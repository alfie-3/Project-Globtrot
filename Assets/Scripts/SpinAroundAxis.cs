using UnityEngine;

public class SpinAroundAxis : MonoBehaviour
{
    [SerializeField] float SpinSpeed = 1f;

    void Update()
    {
        transform.Rotate(Vector3.up, SpinSpeed * Time.deltaTime);    
    }
}
