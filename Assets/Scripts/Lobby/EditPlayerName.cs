using System;
using Imported_Assets.CodeMonkeyLobby.InputWindow.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class EditPlayerName : MonoBehaviour
    {
        public static EditPlayerName Instance { get; private set; }
    
        public event EventHandler OnNameChanged;
    
        [SerializeField] private TextMeshProUGUI playerNameText;
    
        private string _playerName = "PiNickio";

        private void Awake()
        {
            Instance = this;
        
            GetComponent<Button>().onClick.AddListener(() =>
            {
                UIInputWindow.Show_Static("Player Name", _playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
                () => {
                    // Cancel
                },
                (string newName) => {
                    _playerName = newName;

                    playerNameText.text = _playerName;

                    OnNameChanged?.Invoke(this, EventArgs.Empty);
                });
            });
            
            playerNameText.text = _playerName;
        }

        private void Start()
        {
            OnNameChanged += EditPlayerName_OnNameChanged;
        }
        
        private void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
            LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
        }

        public string GetPlayerName() {
            return _playerName;
        }
    }
}
