// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Authoring;
// using UnityEngine;
//
// namespace Physics.Joints
// {
//     public class SpringJointAuthoring: MonoBehaviour, IConvertGameObjectToEntity
//     {
//         [Range(0, Constraint.DefaultSpringFrequency)]
//         public float SpringFrequency = Constraint.DefaultSpringFrequency;
//         [Range(0, Constraint.DefaultSpringDamping)]
//         public float SpringDamping = Constraint.DefaultSpringDamping;
//         
//         [Range(0, Constraint.DefaultSpringFrequency)]
//         public float AngularSpringFrequency = Constraint.DefaultSpringFrequency;
//         [Range(0, Constraint.DefaultSpringDamping)]
//         public float AngularSpringDamping = Constraint.DefaultSpringDamping;
//         
//         public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//         {
//             var rigidBody = new RigidTransform(quaternion.identity, float3.zero);
//             var joint = SimpleSpringJoint.CreateFixed(
//                 rigidBody, 
//                 rigidBody, 
//                 SpringFrequency, 
//                 SpringDamping,
//                 AngularSpringFrequency,
//                 AngularSpringDamping);
//             dstManager.AddComponentData(entity, joint);
//             dstManager.AddComponent<NetworkedJoint>(entity);
//             dstManager.AddComponentData(entity, new PhysicsConstrainedBodyPair(Entity.Null, Entity.Null, false));
//             dstManager.AddSharedComponentManaged(entity, new PhysicsWorldIndex(0));
//         }
//     }
// }