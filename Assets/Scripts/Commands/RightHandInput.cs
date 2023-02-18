using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Commands
{
    [GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
    public struct RightHandInput: IInputComponentData
    {
        [GhostField(Quantization = 1000)] public float3 Position;
        [GhostField(Quantization = 1000)] public quaternion Rotation;
    }
    
}