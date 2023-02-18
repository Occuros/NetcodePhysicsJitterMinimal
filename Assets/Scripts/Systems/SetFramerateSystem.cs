using Unity.Entities;
using Unity.NetCode;

namespace Holonautic.Scripts.NetworkSetup.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [DisableAutoCreation]
    public partial class SetFramerateSystem: SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            if (!SystemAPI.HasSingleton<ClientServerTickRate>())
            {
                var e = EntityManager.CreateEntity();
                EntityManager.AddComponent<ClientServerTickRate>(e);
            }
            var tickRate = SystemAPI.GetSingleton<ClientServerTickRate>();
            tickRate.SimulationTickRate = 72;

            World.GetExistingSystemManaged<PredictedFixedStepSimulationSystemGroup>().RateManager.Timestep = 1f / tickRate.SimulationTickRate;
            SystemAPI.SetSingleton(tickRate);
        }

        protected override void OnUpdate()
        {
        }
    }
}