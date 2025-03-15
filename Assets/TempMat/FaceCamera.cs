using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class FaceCamera : NetworkBehaviour {
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
        if (IsOwner) 
        {
            RotateSprite();
            if (Bounce)
                BounceSprite();
        }
    }

    
    private void BounceSprite() {
        // Use Mathf.Sin to get a smooth oscillating bounce effect
        float bounce = Easing.OutCirc((Mathf.Sin(Time.time * BounceSpeed) + 1) * 0.5f) * BounceHeight;
        // Apply the bounce to the Y position
        transform.position = new Vector3(transform.position.x, startY + bounce, transform.position.z);
    }
    private void RotateSprite() {
        // Get the camera from the local player
        if (playerCamera == null) {
            playerCamera = Camera.main; // Assuming the main camera is the local player's camera
        }

        if (playerCamera != null) {
            // Rotate the sprite to face the camera (Y-axis only for 2D look)
            Vector3 direction = (playerCamera.transform.position - transform.position).normalized;
            direction.y = 0; // Prevent rotating on the Y-axis, which is not needed for 2D
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0); // Rotate only around the Y-axis 
        }
    }

}
