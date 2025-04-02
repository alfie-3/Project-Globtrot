using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameraManager : NetworkBehaviour
{
    [SerializeField] CinemachineCamera camPrefab;
    [field: SerializeField] public Transform Viewpoint { get; private set; }

    public Transform CamTransform {  get; private set; }

    public Action<GameObject> OnCameraAssigned = delegate { };

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

        camera.Follow = Viewpoint;
        CamTransform = camera.transform;
        camera.enabled = true;

        OnCameraAssigned.Invoke(camera.gameObject);
    }

    public void SetPanTilt(Vector2 rotation)
    {
        if (CamTransform.TryGetComponent(out CinemachinePanTilt panTilt))
        {
            panTilt.PanAxis.Value = rotation.x;
            panTilt.TiltAxis.Value = rotation.y;
        }
    }
}
