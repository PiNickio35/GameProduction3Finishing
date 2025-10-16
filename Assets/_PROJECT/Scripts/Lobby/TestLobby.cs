using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _PROJECT.Scripts.Lobby
{
    public class TestLobby : MonoBehaviour
    {
        private Unity.Services.Lobbies.Models.Lobby _hostLobby;
        private Unity.Services.Lobbies.Models.Lobby _joinedLobby;
        private float _heartbeatInterval;
        private float _lobbyUpdateInterval;
        private string _playerName;
        
        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _playerName = "PiNickio" + Random.Range(10, 99);
            
            CreateLobby();
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (_hostLobby != null)
            {
                _heartbeatInterval -= Time.deltaTime;
                if (_heartbeatInterval < 0)
                {
                    float heartbeatIntervalMax = 15;
                    _heartbeatInterval = heartbeatIntervalMax;

                    await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                }
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (_joinedLobby != null)
            {
                _lobbyUpdateInterval -= Time.deltaTime;
                if (_lobbyUpdateInterval < 0)
                {
                    float lobbyUpdateIntervalMax = 1.1f;
                    _lobbyUpdateInterval = lobbyUpdateIntervalMax;

                    Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                    _joinedLobby  = lobby;
                }
            }
        }

        private async void CreateLobby()
        {
            try
            {
                string lobbyName = "Lobby";
                int maxPlayers = 4;
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                        {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Race")}
                    }
                };
                Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                _hostLobby = lobby;
                _joinedLobby = _hostLobby;
                Debug.Log("Lobby created");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
            
                Debug.Log("Lobbies found: " + queryResponse.Results.Count);
                foreach (var lobby in queryResponse.Results)
                {
                    Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
            
        }

        private async void JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };
                Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                _joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void QuickJoinLobby()
        {
            try
            {
                await LobbyService.Instance.QuickJoinLobbyAsync();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName) }
                }
            };
        }

        private void PrintPlayers()
        {
            PrintPlayers(_joinedLobby);
        }

        private void PrintPlayers(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            Debug.Log(lobby.Name + " " + lobby.Data["GameMode"].Value);
            foreach (var player in lobby.Players)
            {
                Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
            }
        }

        private async void UpdateLobbyGameMode(string gameMode)
        {
            try
            {
                _hostLobby = await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
                    }
                });
                _joinedLobby = _hostLobby;
                PrintPlayers(_hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void UpdatePlayerName(string newPlayerName)
        {
            try
            {
                _playerName = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName) }
                    }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void DeleteLobby()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
