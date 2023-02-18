using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Scripting;
using Variants;


namespace DefaultNamespace.Components
{
    [Preserve]
    [GhostComponentVariation(typeof(LocalTransform), "Transform - Precise")]
    [GhostComponent(PrefabType = GhostPrefabType.All, 
        SendDataForChildEntity = false)]
    public struct LocalTransformPrecise
    {
        [GhostField(Quantization = 0, Smoothing = SmoothingAction.InterpolateAndExtrapolate, MaxSmoothingDistance = 1.1f)]
        public float3 Position;
        [GhostField(Quantization = 0, Smoothing = SmoothingAction.InterpolateAndExtrapolate, MaxSmoothingDistance = 1.1f)]
        public quaternion Rotation;
        [GhostField(Quantization = 0, Smoothing = SmoothingAction.InterpolateAndExtrapolate, MaxSmoothingDistance = 1.1f)]
        public float Scale;


    }
    //
    // [GhostComponentVariation(typeof (PhysicsVelocity), "PhysicsVelocity - Precise")]
    // [GhostComponent(PrefabType = GhostPrefabType.All, SendDataForChildEntity = false)]
    // public struct PhysicsVelocityPreciseVariant
    // {
    //     [GhostField(Quantization = 1000, Smoothing = SmoothingAction.InterpolateAndExtrapolate)]
    //     public float3 Linear;
    //     [GhostField(Quantization = 1000, Smoothing = SmoothingAction.InterpolateAndExtrapolate)]
    //     public float3 Angular;
    // }
    //
    
}       