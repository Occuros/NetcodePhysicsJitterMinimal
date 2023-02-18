using Commands;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{

    
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial class MovePlayerFromInputSystemInPhysics : SystemBase
    {

        protected override void OnCreate()
        {
        }


        protected override void OnUpdate()
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            Entities
                .WithAll<Simulate, MoveInPhysicsLoop>()
                .ForEach((Entity entity,
                    ref PhysicsVelocity velocity,
                    in LocalTransform transform,
                    in RightHandInput input,
                    in PredictedGhostComponent prediction) =>
                {
                    
                    var rigidTransform = new RigidTransform()
                    {
                        pos = input.Position,
                        rot = input.Rotation,
                    };
                    var targetVelocity = PhysicsVelocity.CalculateVelocityToTarget( SystemAPI.GetComponent<PhysicsMass>(entity), transform.Position, transform.Rotation,
                        rigidTransform, 1f / deltaTime);

                    velocity = targetVelocity;
                }).Run();
        }
    }
}