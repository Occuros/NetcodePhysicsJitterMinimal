using Commands;
using Components;
using Mono;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class RightHandInputSystem : SystemBase
    {
        private Transform _rightHandGo;
        private Entity _localTrackingSpaceEntity;


        protected override void OnCreate()
        {
            RequireForUpdate<PlayerSpawner>();
            RequireForUpdate<NetworkIdComponent>();
            _rightHandGo = Object.FindObjectOfType<SimulatedRightHand>().transform;
            
        }


        protected override void OnUpdate()
        {
            Entities
               .WithAll<GhostOwnerIsLocal>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref RightHandInput input,
                    in GhostOwnerComponent owner
                ) =>
                {
                    var t = _rightHandGo.transform;
                    input.Position = t.position;
                    input.Rotation = t.rotation;
                    if (!math.all(math.isfinite(input.Position)))
                    {
                        input.Position = float3.zero;
                    }
                }).Run();
        }
    }
}