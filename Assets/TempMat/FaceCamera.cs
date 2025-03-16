using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class FaceCamera : MonoBehaviour {
    // Reference to the camera to face (for local player's camera)
    private Camera playerCamera;

    float startY;
    [SerializeField] private bool Bounce;
    [SerializeField] private float BounceSpeed;
    [SerializeField] private float BounceHeight;

    private void Awake() {
        startY = transform.position.y;

    }
    // Update is called once per frame
    void Update() {
        RotateSprite();
        if (Bounce)
           BounceSprite();
    }

    
    private void BounceSprite() {
        float bounce = Easing.OutCirc((Mathf.Sin(Time.time * BounceSpeed) + 1) * 0.5f) * BounceHeight;
        transform.position = new Vector3(transform.position.x, startY + bounce, transform.position.z);
    }
    private void RotateSprite() {
        if (playerCamera == null) {
            playerCamera = Camera.main;
        }
        if (playerCamera != null) {
            Vector3 direction = (playerCamera.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }
    }

}
