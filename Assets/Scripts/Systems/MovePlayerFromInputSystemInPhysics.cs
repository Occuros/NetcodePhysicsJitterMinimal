using Commands;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Systems
{

    
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public partial class MovePlayerFromInputSystemInPhysics : SystemBase
    {
        private GhostPredictionSystemGroup _ghostPredictionSystemGroup;

        protected override void OnCreate()
        {
            _ghostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            this.RegisterPhysicsRuntimeSystemReadWrite();
        }

        protected override void OnUpdate()
        {
            var tick = _ghostPredictionSystemGroup.PredictingTick;
            var deltaTime = Time.DeltaTime;
            
            Entities
                .WithAll<MoveInPhysicsLoop>()
                .ForEach((Entity entity,
                    ref PhysicsVelocity velocity,
                    in Translation translation,
                    in Rotation rotation,
                    in DynamicBuffer<RightHandInput> inputBuffer,
                    in PredictedGhostComponent prediction) =>
                {
                    if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;

                    inputBuffer.GetDataAtTick(tick, out var input);

                    var rigidTransform = new RigidTransform()
                    {
                        pos = input.Position,
                        rot = input.Rotation,
                    };
                    var targetVelocity = PhysicsVelocity.CalculateVelocityToTarget(GetComponent<PhysicsMass>(entity), translation, rotation,
                        rigidTransform, 1f / deltaTime);

                    velocity = targetVelocity;
                }).Run();
        }
    }
}