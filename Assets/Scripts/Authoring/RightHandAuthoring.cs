using Commands;
using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class RightHandAuthoring : MonoBehaviour
    {
        internal class RightHandBaker : Baker<RightHandAuthoring>
        {
            public override void Bake(RightHandAuthoring authoring)
            {
                AddComponent<RightHandInput>();
                AddComponent<PlayerHand>();
            }
        }
    }
}

