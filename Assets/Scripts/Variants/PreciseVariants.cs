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
    [GhostComponentVariation(typeof(Translation), "Translation - Precise")]
    [GhostComponent(PrefabType = GhostPrefabType.All, OwnerPredictedSendType = GhostSendType.All,
        SendDataForChildEntity = false)]
    public struct TranslationPrecise
    {
        [GhostField(Composite = true, Quantization = 1000, Smoothing = SmoothingAction.InterpolateAndExtrapolate, MaxSmoothingDistance = 0.1f)]
        public float3 Value;

    }

    [Preserve]
    [GhostComponentVariation(typeof(Rotation), "Rotation - Precise")]
    [GhostComponent(PrefabType = GhostPrefabType.All, OwnerPredictedSendType = GhostSendType.All,
        SendDataForChildEntity = false)]
    public struct RotationPrecise
    {
        [GhostField(Quantization = 1000, Smoothing = SmoothingAction.InterpolateAndExtrapolate)]
        public quaternion Value;
    }
    
    [GhostComponentVariation(typeof (PhysicsVelocity), "PhysicsVelocity - Precise")]
    [GhostComponent(OwnerPredictedSendType = GhostSendType.Predicted, PrefabType = GhostPrefabType.All, SendDataForChildEntity = false)]
    public struct PhysicsVelocityPreciseVariant
    {
        [GhostField(Quantization = 1000, Smoothing = SmoothingAction.InterpolateAndExtrapolate)]
        public float3 Linear;
        [GhostField(Quantization = 1000, Smoothing = SmoothingAction.InterpolateAndExtrapolate)]
        public float3 Angular;
    }
    
    
}       