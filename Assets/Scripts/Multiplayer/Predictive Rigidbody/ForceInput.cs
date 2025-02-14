using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;

public struct ForceInput : INetworkSerializable
{
    public Vector3 Force;
    public ForceMode ForceMode;
    public int Tick;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();

            reader.ReadValueSafe(out Tick);
            reader.ReadValueSafe(out Force);
            reader.ReadValueSafe(out ForceMode);

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();

            writer.WriteValueSafe(Tick);
            writer.WriteValueSafe(Force);
            writer.WriteValueSafe(ForceMode);
        }
    }
}

public struct SimulationState : INetworkSerializable
{
    public int Tick;

    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity;
    public Vector3 AngularVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();

            reader.ReadValueSafe(out Tick);
            reader.ReadValueSafe(out Position);
            reader.ReadValueSafe(out Rotation);
            reader.ReadValueSafe(out Velocity);
            reader.ReadValueSafe(out AngularVelocity);

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();

            writer.WriteValueSafe(Position);
            writer.WriteValueSafe(Rotation);
            writer.WriteValueSafe(Velocity);
            writer.WriteValueSafe(AngularVelocity);
        }
    }
}
