using System.Diagnostics;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;


namespace Variants
{
    // [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    // public partial class SmoothingRegisteringSystem : SystemBase
    // {
    //     protected override void OnStartRunning()
    //     {
    //         base.OnStartRunning();
    //         
    //         
    //         if (SystemAPI.TryGetSingleton<GhostPredictionSmoothing>(out var smoothing))
    //         {
    //             smoothing.RegisterSmoothingAction<LocalTransform>(DefaultTraceListener);
    //         }
    //     }
    //
    //
    //     protected override void OnUpdate()
    //     {
    //     }
    // }
}