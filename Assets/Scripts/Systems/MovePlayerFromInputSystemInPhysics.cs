using Commands;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
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

        protected override void OnUpdate()
        {
            var tick = _ghostPredictionSystemGroup.PredictingTick;
            var deltaTime = Time.DeltaTime;
            
            //Handle right hand
            Entities
                .WithAll<MoveInPhysicsLoop>()
                .ForEach((Entity entity,
                    ref Translation translation,
                    ref Rotation rotation,
                    in DynamicBuffer<RightHandInput> inputBuffer,
                    in PredictedGhostComponent prediction) =>
                {
                    if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;

                    inputBuffer.GetDataAtTick(tick, out var input);

                    if (math.any(math.isinf(input.Position)) || math.all(input.Position == float3.zero))
                    {
                        return;
                    }
                    translation.Value = input.Position;
                    rotation.Value = input.Rotation;
                }).Run();
        }
    }
}