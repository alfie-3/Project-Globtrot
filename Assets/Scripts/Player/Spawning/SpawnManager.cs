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

    NetworkVariable<int> spanwedPlayers = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);

    public void Awake()
    {
        NetworkManager.SceneManager.OnLoadComplete += SpawnPlayerOnSceneLoad;

        foreach(CharacterReferenceData playerRef in playerCharacterReferences)
        {
            playerReferenceDict.Add(playerRef.PlayerReferenceID, playerRef);
        }
    }

    public void SpawnPlayerOnSceneLoad(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        RequestSpawnPlayer_Rpc(clientId, PlayerProfile.CharacterReferenceData.PlayerReferenceID);
    }

    [Rpc(SendTo.Server)]
    public void RequestSpawnPlayer_Rpc(ulong id, string playerCharacterReference)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        SpawnPoint spawnPoint = spawnPoints[spanwedPlayers.Value % spawnPoints.Count];
        spanwedPlayers.Value++;


        if (!playerReferenceDict.TryGetValue(playerCharacterReference, out CharacterReferenceData characterReferenceData))
        {
            characterReferenceData = defaultCharacterReference;
        }

        NetworkObject player = Instantiate(characterReferenceData.PlayerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<NetworkObject>();

        player.SpawnAsPlayerObject(id);

        player.transform.position = spawnPoint.transform.position;

        Debug.Log($"Spawned player {id} at {spawnPoint.gameObject.name}");
    }

    public SpawnPoint GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
    }
}
