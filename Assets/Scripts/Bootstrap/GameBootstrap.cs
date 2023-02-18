using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace DefaultNamespace
{
    [UnityEngine.Scripting.Preserve]
    public class GameBootstrap: ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {


            AutoConnectPort = 7979;
            var serverIP = "127.0.0.1";

            DefaultConnectAddress = NetworkEndpoint.Parse(serverIP, 7979);
            
            var result = base.Initialize(defaultWorldName);

            Debug.Log($"We are connecting to the server {serverIP}");
            return result;
        }
        
       
    }
}