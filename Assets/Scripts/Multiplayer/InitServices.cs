using Unity.Netcode;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

public class InitServices : MonoBehaviour
{

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }
}
