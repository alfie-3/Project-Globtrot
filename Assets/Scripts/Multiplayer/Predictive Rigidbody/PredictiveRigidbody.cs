using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PredictiveRigidbody : NetworkBehaviour
{
    NetworkTimer timer;
    const int StateCacheSize = 1024;

    Rigidbody rigidbody;

    //Client
    CircularBuffer<ForceInput> clientPhysicsInputBuffer = new(StateCacheSize);
    CircularBuffer<SimulationState> clientStateBuffer = new(StateCacheSize);
    Queue<ForceInput> pendingForces = new Queue<ForceInput>();
    SimulationState lastSimulationState;

    //Server
    Queue<ForceInput> serverForceInputsQueue = new();
    CircularBuffer<SimulationState> serverStateBuffer = new(StateCacheSize);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        timer = new(NetworkManager.Singleton.NetworkTickSystem.TickRate);
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer?.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (timer == null) return;

        if (timer.ShouldTick())
        {
            HandleClientTick();
            HandleServerTick();
        }
    }

    private void HandleClientTick()
    {
        if (!IsClient) return;

        int currentTick = timer.CurrentTick;
        int bufferIndex = currentTick % StateCacheSize;

        SendToServer_Rpc(pendingForces.ToArray());

        while (pendingForces.Count > 0)
        {
            ForceInput pendingForce = pendingForces.Dequeue();
            rigidbody.AddForce(pendingForce.Force, pendingForce.ForceMode);
        }

        SimulationState simulation = SimulatePhysics();
        clientStateBuffer.Add(simulation, bufferIndex);
    }

    private void HandleServerTick()
    {
        if (!IsServer) return;

        var bufferIndex = -1;
        while (serverForceInputsQueue.Count > 0)
        {
            ForceInput forceInput = serverForceInputsQueue.Dequeue();
            rigidbody.AddForce(forceInput.Force, forceInput.ForceMode);
            bufferIndex = forceInput.Tick % StateCacheSize;

            SimulationState simulationState = SimulatePhysics();
            serverStateBuffer.Add(simulationState, bufferIndex);
        }

        if (bufferIndex == -1) return;

        SendToClient_Rpc(serverStateBuffer.Get(bufferIndex));
    }

    private SimulationState SimulatePhysics()
    {
        Physics.simulationMode = SimulationMode.Script;

        Physics.Simulate(Time.fixedDeltaTime);

        Physics.simulationMode = SimulationMode.FixedUpdate;

        return GetSimulationState();
    }

    [Rpc(SendTo.Server)]
    public void SendToServer_Rpc(ForceInput[] forceInputs)
    {
        foreach (ForceInput forceInput in forceInputs)
        {
            serverForceInputsQueue.Enqueue(forceInput);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SendToClient_Rpc(SimulationState simulationState)
    {
        if (IsOwner)
        {
            lastSimulationState = simulationState;
        }
        else
        {
            transform.SetPositionAndRotation(simulationState.Position, simulationState.Rotation);
            lastSimulationState = simulationState;
        }
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        ForceInput forceInput = new()
        {
            Force = force,
            ForceMode = forceMode
        };

        pendingForces.Enqueue(forceInput);
    }

    public SimulationState GetSimulationState()
    {
        return new SimulationState()
        {
            Tick = timer.CurrentTick % StateCacheSize,
            Position = transform.position,
            Rotation = transform.rotation,
            Velocity = rigidbody.linearVelocity,
            AngularVelocity = rigidbody.angularVelocity,
        };
    }
}
