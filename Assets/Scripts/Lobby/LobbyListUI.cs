using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyListUI : MonoBehaviour
    {
        public static LobbyListUI Instance { get; private set; }

        [SerializeField] private Transform lobbySingleTemplate;
        [SerializeField] private Transform container;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button createLobbyButton;

        private void Awake()
        {
            Instance = this;
        
            lobbySingleTemplate.gameObject.SetActive(false);
        
            refreshButton.onClick.AddListener(RefreshButtonClick);
            createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
        }

        private void Start()
        {
            LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
            LobbyManager.Instance.OnJoinLobby += LobbyManager_OnJoinLobby;
            LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        }
        
        private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e) {
            Show();
        }

        private void LobbyManager_OnLeftLobby(object sender, EventArgs e) {
            Show();
        }

        private void LobbyManager_OnJoinLobby(object sender, LobbyManager.LobbyEventArgs e) {
            Hide();
        }

        private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e) {
            UpdateLobbyList(e.lobbyList);
        }
        
        private void UpdateLobbyList(List<Unity.Services.Lobbies.Models.Lobby> lobbyList) {
            foreach (Transform child in container) {
                if (child == lobbySingleTemplate) continue;

                Destroy(child.gameObject);
            }

            foreach (Unity.Services.Lobbies.Models.Lobby lobby in lobbyList) {
                Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
                lobbySingleTransform.gameObject.SetActive(true);
                LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
                lobbyListSingleUI.UpdateLobby(lobby);
            }
        }

        private void RefreshButtonClick()
        {
            LobbyManager.Instance.RefreshLobbyList();
        }

        private void CreateLobbyButtonClick()
        {
            LobbyCreateUI.Instance.Show();
        }
        
        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Show() {
            gameObject.SetActive(true);
        }
    }
}
