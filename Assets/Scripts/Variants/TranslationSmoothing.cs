using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;


namespace Variants
{
    [UpdateInWorld(TargetWorld.Client)]
    public partial class SmoothingRegisteringSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            var smoothing = World.GetExistingSystem<GhostPredictionSmoothingSystem>();
            if (smoothing != null)
            {
                smoothing.RegisterSmoothingAction<Translation>(DefaultTranslateSmoothingAction.Action);
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}