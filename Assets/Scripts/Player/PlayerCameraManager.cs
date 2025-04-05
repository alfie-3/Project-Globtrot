using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameraManager : NetworkBehaviour
{
    [SerializeField] CinemachineCamera camPrefab;
    [field: SerializeField] public Transform Viewpoint { get; private set; }

    public Transform CamTransform { get; private set; }

    public Action<GameObject> OnCameraAssigned = delegate { };

    CinemachinePanTilt panTilt;
    CinemachineInputAxisController inputController;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        CreateCamera();
    }

    //Created the players camera - Only creates camera for the local players character
    private void CreateCamera()
    {
        if (!IsLocalPlayer) return;

        CinemachineCamera camera = Instantiate(camPrefab);

        camera.TryGetComponent(out panTilt);
        camera.TryGetComponent(out inputController);

        camera.Follow = Viewpoint;
        CamTransform = camera.transform;
        camera.enabled = true;

        OnCameraAssigned.Invoke(camera.gameObject);
    }

    public void SetPanTiltEnabled(bool value)
    {
        inputController.Controllers[0].Enabled = value;
        inputController.Controllers[1].Enabled = value;
    }

    public void SetPanTilt(Vector2 rotation)
    {
        panTilt.PanAxis.Value = rotation.x;
        panTilt.TiltAxis.Value = rotation.y;
    }
}
