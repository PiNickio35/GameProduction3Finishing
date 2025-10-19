using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI playersText;

        private Unity.Services.Lobbies.Models.Lobby _lobby;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                LobbyManager.Instance.JoinLobby(_lobby);
            });
        }

        public void UpdateLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            _lobby = lobby;
            lobbyNameText.text = lobby.Name;
            playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }
    }
}
