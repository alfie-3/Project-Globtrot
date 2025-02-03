using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] bool AutoSpawnPlayers = true;

    [SerializeField] GameObject playerPrefab;

    [SerializeField] SpawnPoint defaultSpawnPoint;
    [SerializeField] List<SpawnPoint> spawnPoints;

    NetworkVariable<int> spanwedPlayers = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);

    public void Start()
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        SpawnPlayer(networkManager.LocalClientId, spawnPoints[spanwedPlayers.Value % spawnPoints.Count]);

        spanwedPlayers.Value++;

    }

    public void SpawnPlayer(ulong id, SpawnPoint spawnPoint)
    {
        NetworkObject player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<NetworkObject>();

        player.SpawnAsPlayerObject(id);

        Debug.Log($"Spawned player {id} at {spawnPoint.gameObject.name}");
    }

    public SpawnPoint GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
    }
}
