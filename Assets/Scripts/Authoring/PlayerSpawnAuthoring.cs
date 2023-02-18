using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PlayerSpawnAuthoring : MonoBehaviour
    {
        public GameObject ItemPrefab;
        public GameObject JointPrefab;
        public GameObject RightHandPrefab;
        public GameObject RightHandPhysicsPrefab;
        internal class PlayerSpawnBaker : Baker<PlayerSpawnAuthoring>
        {
            public override void Bake(PlayerSpawnAuthoring authoring)
            {
                var playerSpawner = new PlayerSpawner()
                {
                    ItemPrefab = GetEntity(authoring.ItemPrefab),
                    JointPrefab = GetEntity(authoring.JointPrefab),
                    RightHandPrefab = GetEntity(authoring.RightHandPrefab),
                    RightHandPhysicsPrefab = GetEntity(authoring.RightHandPhysicsPrefab)
                };
                
                AddComponent(playerSpawner);
            }
        }
    }
}

