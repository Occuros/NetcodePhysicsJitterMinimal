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
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial class ImproveSmoothingForExtrapolationSystem : SystemBase
    {

        protected override void OnCreate()
        {
         
        }

        protected override void OnUpdate()
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var tick = networkTime.ServerTick;
            var deltaTime = SystemAPI.Time.DeltaTime;
            var serverTickFraction = networkTime.ServerTickFraction;
            var isFinalTick = networkTime.IsFinalPredictionTick;
            Entities
                .WithAll<Simulate, MoveInPhysicsLoop>()
                .ForEach((Entity entity,
                    ref PhysicsGraphicalSmoothing graphicalSmoothing,
                    in LocalTransform transform,
                    in RightHandInput input,
                    in PredictedGhostComponent prediction) =>
                {
                    if (serverTickFraction >= 1 || !isFinalTick) return;
                    
                    var rigidTransform = new RigidTransform()
                    {
                        pos = input.Position,
                        rot = input.Rotation,
                    };
                    var targetVelocity = PhysicsVelocity.CalculateVelocityToTarget(SystemAPI.GetComponent<PhysicsMass>(entity),
                        transform.Position, transform.Rotation,
                        rigidTransform, 1f / deltaTime);

                    graphicalSmoothing.ApplySmoothing = 1;
                    graphicalSmoothing.CurrentVelocity = targetVelocity;

                }).Run();
        }
    }
}