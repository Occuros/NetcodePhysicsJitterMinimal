// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.NetCode;
// using UnityEngine;
//
// namespace Physics.Joints
// {
//
//     public class NetworkedJointAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//     {
//         public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//         {
//             dstManager.AddComponent<NetworkedJoint>(entity);
//         }
//     }
//     
//     
//     [GhostComponent()]
//     public struct NetworkedJoint: IComponentData
//     {
//         [GhostField] public Entity ConnectedEntityA;
//         [GhostField] public Entity ConnectedEntityB;
//
//         [GhostField(Quantization = 1000)] public float3 LocalPositionOfA;
//         [GhostField(Quantization = 1000)] public quaternion LocalRotationOfA;
//     }
// }