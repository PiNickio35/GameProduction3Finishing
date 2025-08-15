using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _PROJECT.Scripts
{
    public class PlayerLobbyManager : MonoBehaviour
    {
        private PlayerInputManager _playerLobby;
        [SerializeField] private GameObject playerNamePrefab;
        [SerializeField] private VerticalLayoutGroup groupParent;
        private List<PlayerInput> _listOfJoinedPlayers = new List<PlayerInput>();

        private void Awake()
        {
            _playerLobby = GetComponent<PlayerInputManager>();
            // _playerLobby.onPlayerJoined += AddPlayerToList;
            // _playerLobby.onPlayerLeft += RemovePlayerFromList;
        }

        public void RemovePlayerFromList(PlayerInput obj)
        {
            if (_listOfJoinedPlayers.Contains(obj)) _listOfJoinedPlayers.Remove(obj);
        }

        public void AddPlayerToList(PlayerInput obj)
        {
            Debug.Log("Adding player to list");
            _listOfJoinedPlayers.Add(obj);
            GameObject tempPlayer = Instantiate(playerNamePrefab, groupParent.transform);
            tempPlayer.GetComponent<TextPlayerData>().ConnectPlayerToThisText(obj);
            tempPlayer.GetComponent<TMP_Text>().text = GenerateNewName(obj.GetComponent<PlayerData>().playerName);
        }

        private string GenerateNewName(string incomingName)
        {
            return $"{incomingName} {_listOfJoinedPlayers.Count}";
        }
    }
}
