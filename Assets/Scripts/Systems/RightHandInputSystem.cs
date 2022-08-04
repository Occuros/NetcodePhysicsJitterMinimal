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
        private ClientSimulationSystemGroup _clientSimulationSystemGroup;
        private Transform _rightHandGo;
        private Entity _localTrackingSpaceEntity;


        protected override void OnCreate()
        {
            _clientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
            RequireSingletonForUpdate<PlayerSpawnerComponent>();
            _rightHandGo = Object.FindObjectOfType<SimulatedRightHand>().transform;
        }


        protected override void OnUpdate()
        {
            if (!TryGetSingleton<NetworkIdComponent>(out var networkId)) return;
            var input = default(RightHandInput);

            input.Tick = _clientSimulationSystemGroup.ServerTick;

            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref DynamicBuffer<RightHandInput> rightHandInputBuffer,
                    in GhostOwnerComponent owner
                ) =>
                {
                    if (owner.NetworkId != networkId.Value) return;

                    var t = _rightHandGo.transform;
                    input.Position = t.position;
                    input.Rotation = t.rotation;
                    if (!math.all(math.isfinite(input.Position)))
                    {
                        input.Position = float3.zero;
                    }

                    rightHandInputBuffer.AddCommandData(input);
                }).Run();
        }
    }
}