using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class Belt2 : MonoBehaviour
{
    public float speed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
        Vector3 pos = rigidbody.position;
        rigidbody.position -= transform.forward * speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(pos);
    }
}
