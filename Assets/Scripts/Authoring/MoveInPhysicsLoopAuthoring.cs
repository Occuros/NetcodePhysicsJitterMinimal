using Components;
using Unity.Entities;
using UnityEngine;


public class MoveInPhysicsLoopAuthoring : MonoBehaviour
{
    internal class MoveInPhysicsLoopBaker : Baker<MoveInPhysicsLoopAuthoring>
    {
        public override void Bake(MoveInPhysicsLoopAuthoring authoring)
        {
            AddComponent<MoveInPhysicsLoop>();
        }
    }
}

