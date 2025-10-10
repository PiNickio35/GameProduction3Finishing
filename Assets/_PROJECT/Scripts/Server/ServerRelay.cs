using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace _PROJECT.Scripts.Server
{
    public class ServerRelay : MonoBehaviour
    {
        [SerializeField] private int maxPlayers = 3;
        
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed In {AuthenticationService.Instance.PlayerId}");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async void CreateRelay()
        {
            try
            {
               Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
               string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
               NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                   allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                   allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
               NetworkManager.Singleton.StartHost();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void JoinRelay(string joinCode)
        {
            try
            {
                Debug.Log($"Joining relay with code {joinCode}");
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData,
                    allocation.HostConnectionData);
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
