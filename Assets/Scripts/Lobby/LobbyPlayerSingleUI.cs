using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyPlayerSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image characterImage;
        [SerializeField] private Button kickPlayerButton;
        
        private Player _player;

        private void Awake()
        {
            kickPlayerButton.onClick.AddListener(KickPlayer);
        }

        public void SetKickPlayerButtonVisible(bool visible)
        {
            kickPlayerButton.gameObject.SetActive(visible);
        }

        public void UpdatePlayer(Player player)
        {
            _player = player;
            playerNameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
        }

        private void KickPlayer()
        {
            if (_player != null)
            {
                LobbyManager.Instance.KickPlayer(_player.Id);
            }
        }
    }
}
