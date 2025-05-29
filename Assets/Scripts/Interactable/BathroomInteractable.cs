using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class BathroomInteractable : NetworkBehaviour, IInteractable, IViewable, IEscapeable
{
    [SerializeField] float bathroomDrainRate = 3;

    [SerializeField] Transform sitTargetPos;
    [SerializeField] Transform dismountPos;

    [SerializeField] CinemachineCamera toiletCam;

    [SerializeField] PlayerBathroomHandler currentPlayer;

    [Space]
    [SerializeField] AudioSource toiletAudioSource;
    [SerializeField] AudioClip flushClip;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (currentPlayer != null) return;

        if (interactionManager.TryGetComponent(out PlayerBathroomHandler bathroom))
        {

            if (currentPlayer != null)
            {
                UI_Notifcation.EnqueueNotification("SOMEONE IS USING THIS!");
                return;
            }

            if (bathroom.NormalizedBathroom < 0.33f)
            {
                UI_Notifcation.EnqueueNotification("YOU DONT NEED THIS YET");
                return;
            }

            StartBathroom(bathroom);
        }
    }

    private void Update()
    {
        if (currentPlayer != null)
        {
            currentPlayer.Relieve(bathroomDrainRate * Time.deltaTime);
        }
    }

    public void StartBathroom(PlayerBathroomHandler bathroom)
    {
        Occupy_Rpc(bathroom.NetworkObject);

        currentPlayer = bathroom;
        enabled = true;

        bathroom.GetComponent<PlayerAnimationController>().BeginSit(sitTargetPos.position, sitTargetPos.rotation);
        toiletCam.enabled = true;

        if (toiletCam.TryGetComponent(out CinemachinePanTilt panTilt))
        {
            panTilt.PanAxis.Value = 0f;
            panTilt.TiltAxis.Value = 0f;

            panTilt.enabled = true;
        }

        if (bathroom.TryGetComponent(out PlayerCharacterController chmv))
        {
            chmv.PlayerInputManager.EscapeList.Add(this);
            chmv.CharacterMovement.OnJump += EndBathroom;
            chmv.OnToggledRagdoll += OnRagdollCancel;
        }
    }

    private void OnRagdollCancel(bool obj)
    {
        if (obj == true)
        {
            currentPlayer.GetComponent<PlayerCharacterController>().OnToggledRagdoll -= OnRagdollCancel;
            EndBathroom();
        }
    }

    public void EndBathroom()
    {
        if (currentPlayer == null) return;

        currentPlayer.GetComponent<PlayerAnimationController>().EndSit(dismountPos.position);
        toiletCam.enabled = false;
        enabled = false;

        if (currentPlayer.TryGetComponent(out PlayerCharacterController chmv))
        {
            chmv.CharacterMovement.OnJump -= EndBathroom;
            chmv.OnToggledRagdoll -= OnRagdollCancel;
            chmv.PlayerInputManager.RemoveFromEscapeStack(this);
        }

        if (toiletCam.TryGetComponent(out CinemachinePanTilt panTilt))
        {
            panTilt.PanAxis.Value = 0f;
            panTilt.TiltAxis.Value = 0f;

            panTilt.enabled = false;
        }

        currentPlayer = null;

        UnOccupy_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void Occupy_Rpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject player);

        if (player == null) return;

        player.TryGetComponent(out currentPlayer);
    }

    [Rpc(SendTo.Everyone)]
    public void UnOccupy_Rpc()
    {
        currentPlayer = null;

        toiletAudioSource.PlayOneShot(flushClip, 0.5f);
    }

    public void OnUnview()
    {
        Debug.Log("Unview");
    }

    public InteractionContext OnView()
    {
        return new(true, "Relieve");
    }

    public void Escape(PlayerInputManager manager)
    {
        EndBathroom();
    }
}
