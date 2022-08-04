using Components;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    public partial class GoInGameServerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<PlayerSpawnerComponent>();
            RequireForUpdate(GetEntityQuery(
                ComponentType.ReadOnly<GoInGameRequest>(),
                ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>()));

            if (!HasSingleton<ClientServerTickRate>())
            {
                var e = EntityManager.CreateEntity(typeof(ClientServerTickRate));
            }
            // var tickRate = GetSingleton<ClientServerTickRate>();
            // tickRate.SimulationTickRate = 31;
            // tickRate.SimulationTickRate = 72;
            // tickRate.SimulationTickRate = 90;

            // SetSingleton(tickRate);
        }

        protected override void OnUpdate()
        {
            var spawner = GetSingleton<PlayerSpawnerComponent>();
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            Entities
                .WithNone<SendRpcCommandRequestComponent>()
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity requestEntity, in GoInGameRequest request,
                    in ReceiveRpcCommandRequestComponent received) =>
                {
                    EntityManager.AddComponent<NetworkStreamInGame>(received.SourceConnection);
                    var networkId = GetComponent<NetworkIdComponent>(received.SourceConnection).Value;
                    Debug.Log($"Server setting connection {networkId} to in game");

                    var rightHandEntity = commandBuffer.Instantiate(spawner.RightHandPrefab);
                    commandBuffer.SetComponent(rightHandEntity, new GhostOwnerComponent() { NetworkId = networkId });
                    commandBuffer.AppendToBuffer(received.SourceConnection,
                        new LinkedEntityGroup { Value = rightHandEntity });

                    var rightHandPhysics = commandBuffer.Instantiate(spawner.RightHandPhysicsPrefab);
                    commandBuffer.SetComponent(rightHandPhysics, new GhostOwnerComponent() { NetworkId = networkId });
                    commandBuffer.AppendToBuffer(received.SourceConnection,
                        new LinkedEntityGroup { Value = rightHandPhysics });

                    EntityManager.DestroyEntity(requestEntity);
                }).Run();

            commandBuffer.Playback(EntityManager);
        }
    }
}