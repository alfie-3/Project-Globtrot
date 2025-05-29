using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyStateManager : NetworkBehaviour
{
    public static LobbyStateManager Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static void ForceDisconnectClients()
    {
        if (Instance == null) return;
        if (!Instance.IsServer) return;

        Instance.ForceDisconnect_Rpc();
    }

    [Rpc(SendTo.NotServer)]
    public void ForceDisconnect_Rpc()
    {
        DisconnectAndQuit();
    }

    public void DisconnectAndQuit()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }

    private void OnApplicationQuit()
    {
        ForceDisconnectClients();
    }
}
