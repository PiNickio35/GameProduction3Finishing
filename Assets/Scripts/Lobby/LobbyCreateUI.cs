using Imported_Assets.CodeMonkeyLobby.InputWindow.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyCreateUI : MonoBehaviour
    {
        public static LobbyCreateUI Instance { get; private set; }
        
        [SerializeField] private Button createButton;
        [SerializeField] private Button lobbyNameButton;
        [SerializeField] private Button maxPlayersButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI maxPlayersText;
        
        private string _lobbyName;
        private int _maxPlayers;

        private void Awake()
        {
            Instance = this;
            
            createButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(_lobbyName, _maxPlayers);
                Hide();
            });
            
            lobbyNameButton.onClick.AddListener(() =>
            {
                UIInputWindow.Show_Static("Lobby Name", _lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
                    () => {
                        //Cancel
                    },
                    (string lobbyName) =>
                    {
                        _lobbyName = lobbyName;
                        UpdateText();
                    });
            });
            
            maxPlayersButton.onClick.AddListener(() => {
                UIInputWindow.Show_Static("Max Players", _maxPlayers,
                    () => {
                        // Cancel
                    },
                    (int maxPlayers) => 
                    {
                        _maxPlayers = maxPlayers;
                        if (_maxPlayers > 4) _maxPlayers = 4;
                        if (_maxPlayers < 2) _maxPlayers = 2;
                        UpdateText();
                    });
            });
            
            Hide();
        }

        private void UpdateText()
        {
            lobbyNameText.text = _lobbyName;
            maxPlayersText.text = _maxPlayers.ToString();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _lobbyName = "MyLobby";
            _maxPlayers = 4;
            UpdateText();
        }
    }
}
