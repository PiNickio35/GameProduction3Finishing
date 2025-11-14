using System;
using System.Collections.Generic;
using Server;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }
        
        public const string KEY_PLAYER_NAME = "PlayerName";
        private const string KEY_START_GAME = "StartGame";
        
        public event EventHandler OnLeftLobby;
        public event EventHandler OnGameStarted;
        
        public event EventHandler<LobbyEventArgs> OnJoinLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
        public class LobbyEventArgs : EventArgs
        {
            public Unity.Services.Lobbies.Models.Lobby lobby;
        }
        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs
        {
            public List<Unity.Services.Lobbies.Models.Lobby> lobbyList;
        }

        private Unity.Services.Lobbies.Models.Lobby _joinedLobby;
        private float _heartbeatInterval;
        private float _lobbyUpdateInterval;
        private string _playerName;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
        }
        
        public async void Authenticate(string playerName) {
            _playerName = playerName;
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
                RefreshLobbyList();
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (IsLobbyHost())
            {
                _heartbeatInterval -= Time.deltaTime;
                if (_heartbeatInterval < 0)
                {
                    float heartbeatIntervalMax = 15;
                    _heartbeatInterval = heartbeatIntervalMax;

                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
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

                    _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                    
                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby});

                    if (!IsPlayerInLobby())
                    {
                        Debug.Log("Kicked from lobby!");
                        OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                        _joinedLobby = null;
                    }

                    if (_joinedLobby != null && _joinedLobby.Data[KEY_START_GAME].Value != "0")
                    {
                        if (!IsLobbyHost())
                        {
                            ServerRelay.Instance.JoinRelay(_joinedLobby.Data[KEY_START_GAME].Value);
                        }
                        _joinedLobby = null;
                        OnGameStarted?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public Unity.Services.Lobbies.Models.Lobby GetJoinedLobby()
        {
            return _joinedLobby;
        }

        public bool IsLobbyHost()
        {
            return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private bool IsPlayerInLobby()
        {
            if (_joinedLobby != null && _joinedLobby.Players != null)
            {
                foreach (var player in _joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public Player GetPlayer()
        {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)}
            });
        }

        public async void CreateLobby(string lobbyName, int maxPlayers)
        {
            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")},
                    }
                };
                Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
                _joinedLobby = lobby;
                OnJoinLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
                Debug.Log("Created lobby" + lobby.Name);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void RefreshLobbyList()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions
                {
                    Count = 25,
                    // Filter for open lobbies only.
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    // Order by newest lobbies first.
                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };

                QueryResponse lobbyListQueryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
                {
                    Player = GetPlayer()
                });
                OnJoinLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void UpdatePlayerName(string newPlayerName)
        {
            _playerName = newPlayerName;
            if (_joinedLobby == null) return;
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)}
                    }
                };

                _joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId, options);
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby});
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void LeaveLobby()
        {
            if (_joinedLobby == null) return;
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void KickPlayer(string playerId)
        {
            if (!IsLobbyHost()) return;
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void StartGame()
        {
            if (!IsLobbyHost()) return;
            try
            {
                Debug.Log("Starting game");
                string relayCode = await ServerRelay.Instance.CreateRelay();
                Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(
                    _joinedLobby.Id, new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                        }
                    });
                _joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
