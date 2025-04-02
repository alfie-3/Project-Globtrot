using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] CharacterReferenceData defaultCharacterReference;

    [SerializeField] List<CharacterReferenceData> playerCharacterReferences;
    Dictionary<string, CharacterReferenceData> playerReferenceDict = new();

    [SerializeField] SpawnPoint defaultSpawnPoint;
    [SerializeField] List<SpawnPoint> spawnPoints;

    int spawnPoint = -1;

    NetworkVariable<int> spanwedPlayers = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);

    public static SpawnManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        foreach(CharacterReferenceData playerRef in playerCharacterReferences)
        {
            playerReferenceDict.Add(playerRef.PlayerReferenceID, playerRef);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = null;
    }

    protected override void OnNetworkPostSpawn()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        RequestSpawnPlayer_Rpc(NetworkManager.Singleton.LocalClientId, defaultCharacterReference.PlayerReferenceID);
    }

    public void SpawnPlayerOnSceneLoad(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        RequestSpawnPlayer_Rpc(clientId, PlayerProfile.CharacterReferenceData.PlayerReferenceID);
    }

    [Rpc(SendTo.Server)]
    public void RequestSpawnPlayer_Rpc(ulong id, string playerCharacterReference)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        SpawnPoint spawnPoint = spawnPoints[spanwedPlayers.Value % spawnPoints.Count];
        int selectedSpawnPoint = spanwedPlayers.Value % spawnPoints.Count;
        spanwedPlayers.Value++;


        if (!playerReferenceDict.TryGetValue(playerCharacterReference, out CharacterReferenceData characterReferenceData))
        {
            characterReferenceData = defaultCharacterReference;
        }

        NetworkObject player = Instantiate(characterReferenceData.PlayerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<NetworkObject>();

        player.SpawnAsPlayerObject(id);

        player.transform.position = spawnPoint.transform.position;

        SetSpawnPoint_Rpc(selectedSpawnPoint, RpcTarget.Single(id, RpcTargetUse.Temp));

        Debug.Log($"Spawned player {id} at {spawnPoint.gameObject.name}");
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SetSpawnPoint_Rpc(int spawnPoint, RpcParams rpcParams)
    {
        this.spawnPoint = spawnPoint;
    }

    public void RespawnPlayer(PlayerCharacterController movement)
    {
        SpawnPoint playersSpawnPoint = spawnPoints[spawnPoint % spawnPoints.Count];
        movement.CharacterMovement.Teleport(playersSpawnPoint.transform.position);
        movement.CameraManager.SetPanTilt(new(transform.eulerAngles.x, transform.eulerAngles.y));
    }

    public SpawnPoint GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
    }
}
