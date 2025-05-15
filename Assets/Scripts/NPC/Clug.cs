using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Clug : MonoBehaviour, IOnDrop, IOnHeld
{
    public NavMeshAgent agent {  get; private set; }
    Vector3 lastpos;
    public Vector3 Target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        
        agent.enabled = true;
        ClugManager.Instance.AddClug(this);
    }

    public void GoToTarget()
    {
        if(agent.enabled)
            agent.SetDestination(Target);
    }

    
    public void OnHeld(PlayerHoldingManager manager)
    {
        agent.enabled = false;
    }
    public void OnDrop(PlayerHoldingManager manager)
    {
        GetComponent<RigidbodyNetworkTransform>().OnSleepingChanged += rigidbodyWake;
    }

    protected void rigidbodyWake(bool sleeping)
    {
        if (sleeping)
        {
            agent.enabled = true;
            GetComponent<RigidbodyNetworkTransform>().OnSleepingChanged -= rigidbodyWake;
        }
    }

    public Vector3 GetLastPos() { Vector3 output = lastpos; lastpos = transform.position; return output; }

    private void OnDestroy()
    {
        ClugManager.Instance.RemoveClug(this);
    }

    
}
