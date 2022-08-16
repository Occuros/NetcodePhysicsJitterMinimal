using Components;
using Physics.Joints;
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
    public partial class HandleFixedJointSystem : SystemBase
    {
        private GhostPredictionSystemGroup _ghostPredictionSystemGroup;
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<PlayerSpawnerComponent>();
            RequireSingletonForUpdate<CommandTargetComponent>();
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
            Entities
                .WithAll<PhysicsConstrainedBodyPair>()
                .ForEach((Entity jointEntity,
                    ref NetworkedJoint fixedJoint,
                    in PredictedGhostComponent prediction
                ) =>
                {
                    if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;
                    var constrainedBodyPair = GetComponent<PhysicsConstrainedBodyPair>(jointEntity);
            
                    if (fixedJoint.ConnectedEntityA == constrainedBodyPair.EntityA &&
                        fixedJoint.ConnectedEntityB == constrainedBodyPair.EntityB) return;
            
                    var entityA = fixedJoint.ConnectedEntityA;
                    var entityB = fixedJoint.ConnectedEntityB;
                    var shouldBeNull = fixedJoint.ConnectedEntityA == Entity.Null ||
                                       fixedJoint.ConnectedEntityB == Entity.Null;
                    var eitherIsNot = constrainedBodyPair.EntityA != Entity.Null ||
                                      constrainedBodyPair.EntityB != Entity.Null;
            
                    if (shouldBeNull && eitherIsNot)
                    {
                        SetComponent(jointEntity, new PhysicsConstrainedBodyPair(Entity.Null, Entity.Null, false));
                        return;
                    }
            
                    if (shouldBeNull)
                    {
                        SetComponent(jointEntity, new PhysicsConstrainedBodyPair(Entity.Null, Entity.Null, false));
                        return;
                    }
            
                    var ltwA = GetComponent<LocalToWorld>(entityA);
                    var ltwB = GetComponent<LocalToWorld>(entityB);
            
                    var worldFromA = Math.DecomposeRigidBodyTransform(ltwA.Value);
                    var worldFromB = Math.DecomposeRigidBodyTransform(ltwB.Value);
            
                    var localPosition = fixedJoint.LocalPositionOfA;
                    var localRotation = fixedJoint.LocalRotationOfA;
            
                    var bFromA = math.mul(math.inverse(worldFromB), worldFromA);
                    var aAnchorRotationInWorldOfB = math.mul(bFromA.rot, localRotation);
                    var aAnchorPositionInWorldOfB = math.transform(bFromA, localPosition);
            
                    var joint = GetComponent<PhysicsJoint>(jointEntity);
                    // joint.BodyAFromJoint = new RigidTransform(localRotation, localPosition);
                    // joint.BodyBFromJoint = new RigidTransform(aAnchorRotationInWorldOfB, aAnchorPositionInWorldOfB);
                    joint.BodyAFromJoint = new RigidTransform();
                    joint.BodyBFromJoint = new RigidTransform();
                    
                    SetComponent(jointEntity, new PhysicsConstrainedBodyPair(entityA, entityB, false));
                    SetComponent(jointEntity, joint);
                }).Schedule();
        }
    }
}