using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] bool AutoSpawnPlayers = true;

    [SerializeField] PlayerInputManager playerPrefab;

    [SerializeField] SpawnPoint defaultSpawnPoint;
    [SerializeField] List<SpawnPoint> spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (AutoSpawnPlayers)
            SpawnPlayers();
    }

    public void SpawnPlayers()
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        Queue<SpawnPoint> spawnPointsQueue = new Queue<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in spawnPoints) spawnPointsQueue.Enqueue(spawnPoint);

        foreach (NetworkClient networkClient in networkManager.ConnectedClientsList)
        {
            SpawnPlayer(networkClient.ClientId, spawnPointsQueue.Count > 0 ? spawnPointsQueue.Dequeue() : defaultSpawnPoint);
        }
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
