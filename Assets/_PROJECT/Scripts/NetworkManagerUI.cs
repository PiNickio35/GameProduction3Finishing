using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _PROJECT.Scripts
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button clientButton, serverButton, hostButton;

        private void Awake()
        {
            serverButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });
            clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });
            hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
        }
    }
}
