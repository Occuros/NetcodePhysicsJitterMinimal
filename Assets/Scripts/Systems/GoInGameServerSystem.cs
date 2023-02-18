using Components;
using DefaultNamespace;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct GoInGameServerSystem : ISystem
    {
        private EntityQuery _newNetworkStreamConnectionsQuery;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpawner>();
            state.RequireForUpdate<ReceiveRpcCommandRequestComponent>();

            using var builder = new EntityQueryBuilder(Allocator.Temp)
                               .WithAll<NetworkStreamConnection>()
                               .WithNone<ConnectionState>();
            _newNetworkStreamConnectionsQuery = state.GetEntityQuery(builder);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                               .CreateCommandBuffer(state.WorldUnmanaged);

            state.EntityManager.AddComponent<ConnectionState>(_newNetworkStreamConnectionsQuery);

            var networkIdLookup = SystemAPI.GetComponentLookup<NetworkIdComponent>(true);
            var spawner = SystemAPI.GetSingleton<PlayerSpawner>();
            var netDbg = SystemAPI.GetSingleton<NetDebug>();

            var job = new GoInGameServerJob()
            {
                ecb = ecb,
                networkIdLookup = networkIdLookup,
                spawner = spawner,
            };

            job.Run();
        }

        [BurstCompile]
        private partial struct GoInGameServerJob : IJobEntity
        {
            public EntityCommandBuffer ecb;

            [ReadOnly]
            public ComponentLookup<NetworkIdComponent> networkIdLookup;

            [ReadOnly]
            public PlayerSpawner spawner;


            private void Execute(Entity entity, in GoInGameRequest request,
                                 in ReceiveRpcCommandRequestComponent received)
            {
                ecb.AddComponent<NetworkStreamInGame>(received.SourceConnection);
                if (!networkIdLookup.HasComponent(received.SourceConnection))
                {
                    Debug.LogError("NeworkId is not present on the source connection of the rpc");
                    return;
                }

                var networkId = networkIdLookup[received.SourceConnection].Value;
                Debug.Log($"Server setting connection {networkId} to in game");
                var rightHandEntity = ecb.Instantiate(spawner.RightHandPrefab);
                ecb.SetComponent(rightHandEntity, new GhostOwnerComponent() { NetworkId = networkId });
                ecb.AppendToBuffer(received.SourceConnection,
                    new LinkedEntityGroup { Value = rightHandEntity });

                var rightHandPhysics = ecb.Instantiate(spawner.RightHandPhysicsPrefab);
                ecb.SetComponent(rightHandPhysics, new GhostOwnerComponent() { NetworkId = networkId });
                ecb.AppendToBuffer(received.SourceConnection,
                    new LinkedEntityGroup { Value = rightHandPhysics });


                var itemEntity = ecb.Instantiate(spawner.ItemPrefab);

                ecb.DestroyEntity(entity);
            }
        }
    }
}

