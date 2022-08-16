using Commands;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.GraphicsIntegration;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    
    [UpdateInWorld(TargetWorld.Client)]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(PredictedPhysicsSystemGroup))]
    public partial class ImproveSmoothingForExtrapolationSystem : SystemBase
    {
        private GhostPredictionSystemGroup _ghostPredictionSystemGroup;
        private ClientSimulationSystemGroup _clientSimulationSystemGroup;

        protected override void OnCreate()
        {
            _ghostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
            _clientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
        }

        protected override void OnUpdate()
        {
            var tick = _ghostPredictionSystemGroup.PredictingTick;
            var deltaTime = Time.DeltaTime;
            var serverTickFraction = _clientSimulationSystemGroup.ServerTickFraction;
            var isFinalTick = _ghostPredictionSystemGroup.IsFinalPredictionTick;
            Entities
                .WithAll<MoveInPhysicsLoop>()
                .ForEach((Entity entity,
                    ref PhysicsGraphicalSmoothing graphicalSmoothing,
                    in Translation translation,
                    in Rotation rotation,
                    in DynamicBuffer<RightHandInput> inputBuffer,
                    in PredictedGhostComponent prediction) =>
                {
                    if (serverTickFraction >= 1 || !isFinalTick) return;

                    inputBuffer.GetDataAtTick(tick, out var input);

                    var rigidTransform = new RigidTransform()
                    {
                        pos = input.Position,
                        rot = input.Rotation,
                    };
                    var targetVelocity = PhysicsVelocity.CalculateVelocityToTarget(GetComponent<PhysicsMass>(entity),
                        translation, rotation,
                        rigidTransform, 1f / deltaTime);

                    graphicalSmoothing.ApplySmoothing = 1;
                    graphicalSmoothing.CurrentVelocity = targetVelocity;

                }).Run();
        }
    }
}