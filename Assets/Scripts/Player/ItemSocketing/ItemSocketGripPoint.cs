using UnityEngine;

public class ItemSocketGripPoint : MonoBehaviour
{
    [SerializeField] PlayerObjectSocketManager.ObjectSocket gripSocket;

    public PlayerObjectSocketManager.ObjectSocket GetGripSocket() => gripSocket;
}
