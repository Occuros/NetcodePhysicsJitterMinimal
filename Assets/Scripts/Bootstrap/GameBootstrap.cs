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
            RpcSystem.DynamicAssemblyList = false;


            AutoConnectPort = 7979;
            var serverIP = "127.0.0.1";

            DefaultConnectAddress = NetworkEndPoint.Parse(serverIP, 7979);
            
            var result = base.Initialize(defaultWorldName);
            
#if UNITY_EDITOR
            var addr = RequestedAutoConnect;
                serverIP = addr;
#endif
            
            Debug.Log($"We are connecting to the server {serverIP}");
            return result;
        }
        
       
    }
}