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
        protected override void RegisterDefaultVariants(Dictionary<ComponentType, Type> defaultVariants)
        {
            
            defaultVariants.Add(new ComponentType(typeof(Translation)),typeof(TranslationPrecise));
            defaultVariants.Add(new ComponentType(typeof(Rotation)), typeof(RotationPrecise));
            defaultVariants.Add(new ComponentType(typeof(PhysicsVelocity)), typeof(PhysicsVelocityPreciseVariant));
            
            // defaultVariants.Add(new ComponentType(typeof(Translation)),typeof(TranslationDefaultVariant));
            // defaultVariants.Add(new ComponentType(typeof(Rotation)), typeof(RotationDefaultVariant));
            // defaultVariants.Add(new ComponentType(typeof(PhysicsVelocity)), typeof(PhysicsVelocityDefaultVariant));
        }
    }
}