using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] PlayerCameraManager playerCameraManager;

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
