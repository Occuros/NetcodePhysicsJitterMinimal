using Unity.Entities;

namespace Components
{
    public struct PlayerSpawner: IComponentData
    {
        public Entity RightHandPrefab;
        public Entity RightHandPhysicsPrefab;
        public Entity JointPrefab;
        public Entity ItemPrefab;
    }
}