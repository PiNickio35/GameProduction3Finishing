using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Server
{
    public class ServerRelay : NetworkBehaviour
    {
        public static ServerRelay Instance;
        
        [SerializeField] private int maxPlayers = 4;

        private void Awake()
        {
            Instance = this;
        }

        public async Task<string> CreateRelay()
        {
            try
            {
               Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
               string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
               Debug.Log(joinCode);
               NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("dtls"));
               NetworkManager.Singleton.StartHost();
               return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return null;
            }
        }

        public async void JoinRelay(string joinCode)
        {
            try
            {
                Debug.Log($"Joining relay with code {joinCode}");
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("dtls"));
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        [ServerRpc]
        public void WinServerRpc(string playerName)
        {
            WinClientRpc(playerName);
        }

        [ClientRpc]
        private void WinClientRpc(string playerName)
        {
            LapManager.Instance.winText.GetComponentInChildren<TextMeshProUGUI>().text = "You Win " + playerName + "!";
            LapManager.Instance.winText.gameObject.SetActive(true);
            LapManager.Instance.winSound.Play();
            LapManager.Instance.startYourEngines = false;
            if (!IsHost) return;
            LapManager.Instance.StartCoroutine(LapManager.Instance.WindDown());
        }

        [ServerRpc]
        public void StartCountDownServerRpc()
        {
            StartCountDownClientRpc();
        }

        [ClientRpc]
        public void StartCountDownClientRpc()
        {
            LapManager.Instance.StartCoroutine(LapManager.Instance.CountDown());
        }
    }
}
