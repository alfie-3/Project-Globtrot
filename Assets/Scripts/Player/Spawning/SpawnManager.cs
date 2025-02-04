using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] SpawnPoint defaultSpawnPoint;
    [SerializeField] List<SpawnPoint> spawnPoints;

    NetworkVariable<int> spanwedPlayers = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);

    public void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        NetworkManager.SceneManager.OnLoadComplete += SpawnPlayers;
    }

    public void SpawnPlayers(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        foreach (NetworkClient networkClient in networkManager.ConnectedClientsList)
        {
            if (networkClient.ClientId != clientId) continue;

            SpawnPlayer(clientId, spawnPoints[spanwedPlayers.Value % spawnPoints.Count]);
        }

        spanwedPlayers.Value++;

    }

    public void SpawnPlayer(ulong id, SpawnPoint spawnPoint)
    {
        NetworkObject player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<NetworkObject>();

        player.SpawnAsPlayerObject(id);

        player.transform.position = spawnPoint.transform.position;

        Debug.Log($"Spawned player {id} at {spawnPoint.gameObject.name}");
    }

    public SpawnPoint GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
    }
}
