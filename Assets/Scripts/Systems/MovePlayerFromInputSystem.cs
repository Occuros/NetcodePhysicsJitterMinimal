using Commands;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class MovePlayerFromInputSystem : SystemBase
    {

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            //Handle right hand
            Entities
               .WithAll<Simulate>()
                .WithNone<MoveInPhysicsLoop>()
                .ForEach((Entity entity,
                ref LocalTransform transform,
                in RightHandInput input,
                in PredictedGhostComponent prediction) =>
            {


                if (math.any(math.isinf(input.Position)) || math.all(input.Position == float3.zero))
                {
                    return;
                }
                transform.Position = input.Position;
                transform.Rotation = input.Rotation;
            }).Run();

          
        }
    }
    
   
}