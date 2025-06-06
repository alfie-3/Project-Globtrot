using DG.Tweening;
using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCameraManager : NetworkBehaviour
{
    [SerializeField] CinemachineCamera camPrefab;
    [SerializeField] CinemachineCamera freeCamPrefab;
    [field: Space]
    [field: SerializeField] public Transform Viewpoint { get; private set; }
    [Space]
    [field: SerializeField] CinemachineCamera ragdollCamera;

    public Transform CamTransform { get; private set; }

    public Action<GameObject> OnCameraAssigned = delegate { };

    CinemachineCamera cineCamera;
    CinemachinePanTilt panTilt;
    CinemachineInputAxisController inputController;

    public float defaultFov;
    public Tweener currentFOVTweener;

    public GameObject freeCam = null;

    private void OnEnable()
    {
       if (transform.root.TryGetComponent(out PlayerCharacterController characterController))
        {
            characterController.OnSprintingChanged += ToggleSprintFOV;
            characterController.OnToggledRagdoll += OnToggleRagdoll;
        }

       if (transform.root.TryGetComponent(out PlayerInputManager inputManager))
        {
            inputManager.OnToggleFreeCam += (ctx) => ToggleFreeCam(ctx, inputManager);
        }
    }

    private void OnToggleRagdoll(bool value)
    {
        if (ragdollCamera == null) return;
        if (!IsOwner) return;

        SetHeadcamEnabled(value); 
    }

    public void SetHeadcamEnabled(bool value)
    {
        ragdollCamera.enabled = value;
        panTilt.enabled = !value;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        CreateCamera();
    }

    //Created the players camera - Only creates camera for the local players character
    private void CreateCamera()
    {
        if (!IsLocalPlayer) return;

        cineCamera = Instantiate(camPrefab);

        cineCamera.TryGetComponent(out panTilt);
        cineCamera.TryGetComponent(out inputController);

        defaultFov = cineCamera.Lens.FieldOfView;

        cineCamera.Follow = Viewpoint;
        CamTransform = cineCamera.transform;
        cineCamera.enabled = true;

        OnCameraAssigned.Invoke(cineCamera.gameObject);

        cineCamera.GetComponentInChildren<VignetteEffectController>().Init(transform.root.gameObject);
    }

    public void ToggleSprintFOV(bool value)
    {
        float targetFov = value ? defaultFov * 1.05f : defaultFov;

        currentFOVTweener.Kill();
        currentFOVTweener = DOVirtual.Float(cineCamera.Lens.FieldOfView, targetFov, 0.1f, fov => cineCamera.Lens.FieldOfView = fov);
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

    private void ToggleFreeCam(UnityEngine.InputSystem.InputAction.CallbackContext context, PlayerInputManager inputManager)
    {
        if (freeCam != null)
        {
            Destroy(freeCam);
            inputManager.SetInputEnabled(true);
            SetPanTiltEnabled(true);
            transform.root.GetComponentInChildren<PlayerMultiplayerMaterialsController>().SetMeshCulling(true);
        }
        else
        {
            freeCam = Instantiate(freeCamPrefab, transform.position, transform.rotation).gameObject;
            inputManager.SetInputEnabled(false);
            SetPanTiltEnabled(false);
            transform.root.GetComponentInChildren<PlayerMultiplayerMaterialsController>().SetMeshCulling(false);
        }
    }
}
