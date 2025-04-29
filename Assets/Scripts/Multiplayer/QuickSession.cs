using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using static SessionManager;

public class QuickSession : MonoBehaviour
{
    public static ISession Session { get; private set; }
    public static ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public UnityEvent OnConnect;
    public UnityEvent OnDisconnect;

    public void LocalJoin()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
        NetworkManager.Singleton.StartHost();

        OnConnect.Invoke();
    }
}
