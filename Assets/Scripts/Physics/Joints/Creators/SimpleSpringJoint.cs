using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Physics.Authoring
{
    public class SimpleSpringJoint : BallAndSocketJoint
    {
        [Space]
        [SerializeField] private float3 _slidingAxis;
        [SerializeField] private float _minDistance;
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _springPosition;
        [SerializeField] [Range(0, 74341.31f)] private float SprintForce;
        [SerializeField] [Range(0, 2530.126f)] private float SprintDamping;
        private float3 _axisInConnectedEntity;
        private float3 _perpendicularAxisInConnectedEntity;
        private float3 _perpendicularAxisLocal;


        public override void UpdateAuto()
        {
            base.UpdateAuto();
            if (AutoSetConnected)
            {
                RigidTransform bFromA = math.mul(math.inverse(worldFromB), worldFromA);
                _axisInConnectedEntity = math.mul(bFromA.rot, _slidingAxis);
                _perpendicularAxisLocal = math.up();

                //create an automatic perpendicular axis through he cross product
                _perpendicularAxisLocal = math.@select(
                    math.cross(math.up(), _slidingAxis),
                    math.cross(math.right(), _slidingAxis),
                    math.all(_slidingAxis == math.up())
                );

                _perpendicularAxisInConnectedEntity = math.mul(bFromA.rot, _perpendicularAxisLocal);
            }
        }

        public override void Create(EntityManager entityManager, GameObjectConversionSystem conversionSystem)
        {
            UpdateAuto();

            var joint = PhysicsJoint.CreatePrismatic(
                new BodyFrame
                {
                    Axis = _slidingAxis,
                    PerpendicularAxis = _perpendicularAxisLocal,
                    Position = PositionLocal
                },
                new BodyFrame
                {
                    Axis = _axisInConnectedEntity,
                    PerpendicularAxis = _perpendicularAxisInConnectedEntity,
                    Position = PositionInConnectedEntity
                },
                new Math.FloatRange(_minDistance, _maxDistance)
            );
            var constraints = joint.GetConstraints();
            
            var springConstraint = new Constraint
            {
                // Choose a small damping value instead of 0 to improve stability of the joints

                Max = _springPosition,
                Min = _springPosition,
                SpringDamping = SprintDamping,
                SpringFrequency = SprintForce,
                ConstrainedAxes = new bool3(true, false, false)
            };
            constraints.Add(springConstraint);

            joint.SetConstraints(constraints);

            conversionSystem.World.GetOrCreateSystem<EndJointConversionSystem>().CreateJointEntity(
                this,
                GetConstrainedBodyPair(conversionSystem),
                joint
            );
        }


        public static PhysicsJoint CreateFixed(
            BodyFrame bodyAFromJoint, 
            BodyFrame bodyBFromJoint, 
            float springFrequency = Constraint.DefaultSpringFrequency, 
            float springDamping = Constraint.DefaultSpringDamping,
            float angularSpringFrequency = Constraint.DefaultSpringFrequency,
            float angularSpringDamping = Constraint.DefaultSpringDamping)
        {


            var joint = new PhysicsJoint()
            {
                BodyAFromJoint = bodyAFromJoint,
                BodyBFromJoint = bodyBFromJoint,
                JointType = JointType.Custom
            };
            var constraints = new FixedList128Bytes<Constraint>
            {
                Length = 2,
                [0] = Constraint.BallAndSocket(springFrequency, springDamping),
                [1] = Constraint.FixedAngle(angularSpringFrequency, angularSpringDamping)
            };
            
            joint.SetConstraints(constraints);

            return joint;
        }
    }
}