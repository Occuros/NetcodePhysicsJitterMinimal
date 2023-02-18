using System;
using System.Collections.Generic;
using DefaultNamespace.Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public class DefaultVariantSystem: DefaultVariantSystemBase
    {

        protected override void RegisterDefaultVariants(Dictionary<ComponentType, Rule> defaultVariants)
        {
            defaultVariants.Add(new ComponentType(typeof(LocalTransform)), Rule.ForAll(typeof(LocalTransformPrecise)));

        }
    }
}