using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PlayerSpawnerComponent: IComponentData
    {
        public Entity RightHandPrefab;
        public Entity RightHandPhysicsPrefab;
        public Entity JointPrefab;
        public Entity ItemPrefab;
    }
}