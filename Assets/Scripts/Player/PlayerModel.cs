using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] PlayerCameraManager playerCameraManager;

    [Header("Bones")]
    public GameObject Head;
    public GameObject legHigher_R;
    public GameObject legLower_R;
    public GameObject legHigher_L;
    public GameObject legLower_L;

    private void Update()
    {
        RotateToFaceCameraDirection();
    }

    public void RotateToFaceCameraDirection()
    {
        if (playerCameraManager == null) return;
        if (playerCameraManager.CamTransform == null) return;

        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, playerCameraManager.CamTransform.eulerAngles.y, transform.eulerAngles.z);
    }
}
