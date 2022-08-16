using Components;
using DefaultNamespace;
using Physics.Joints;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    // [AlwaysUpdateSystem]
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

            var tickRate = GetSingleton<ClientServerTickRate>();
            // tickRate.SimulationTickRate = 31;
            tickRate.SimulationTickRate = 72;
            // tickRate.SimulationTickRate = 90;

            SetSingleton(tickRate);
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


                    var itemEntity = commandBuffer.Instantiate(spawner.ItemPrefab);
                    // commandBuffer.SetComponent(itemEntity, new Translation() { Value = new float3(0.0f, -0.02f, 0.0f) });

                    // var jointEntity = commandBuffer.Instantiate(spawner.JointPrefab);


                    var joint = GetComponent<NetworkedJoint>(spawner.RightHandPhysicsPrefab);
                    joint.ConnectedEntityA = rightHandPhysics;
                    joint.ConnectedEntityB = itemEntity;
                    joint.LocalPositionOfA = float3.zero;
                    joint.LocalRotationOfA = quaternion.identity;
                    commandBuffer.SetComponent(rightHandPhysics, joint);
                    Debug.Log($"we can connect joint to {rightHandPhysics.Index}");


                    EntityManager.DestroyEntity(requestEntity);
                }).Run();

            commandBuffer.Playback(EntityManager);
        }
    }
}