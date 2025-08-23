






using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class PhotonManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IMatchmakingCallbacks
    {
        [Header("Photon Settings")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        public bool autoJoinLobby = true;

        private List<PlayerData> playersInRoom = new List<PlayerData>();

        void Awake()
        {
            // Ensure we don't have multiple PhotonManager instances
            if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClient != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            // Initialize Photon
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.AutomaticallySyncScene = true;

            if (autoJoinLobby)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        void Start()
        {
            Debug.Log("PhotonManager started");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master Server");

            if (autoJoinLobby)
            {
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Photon Lobby");
            CreateOrJoinRoom();
        }

        private void CreateOrJoinRoom()
        {
            // Try to join a random room first
            PhotonNetwork.JoinRandomRoom();

            // If no rooms are available, create a new one
            if (PhotonNetwork.CurrentLobby != null)
            {
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"No random room available. Creating a new room: {message}");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created new room");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name} with {PhotonNetwork.PlayerList.Length} players");

            // Load the multiplayer scene
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MultiplayerScene");
            }

            // Add this player to the local list
            PlayerData newPlayer = new PlayerData
            {
                ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber,
                Nickname = PhotonNetwork.LocalPlayer.NickName,
                IsLocalPlayer = true
            };

            playersInRoom.Add(newPlayer);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Player entered room: {newPlayer.NickName} (ActorNumber: {newPlayer.ActorNumber})");

            // Add the player to our local list
            PlayerData player = new PlayerData
            {
                ActorNumber = newPlayer.ActorNumber,
                Nickname = newPlayer.NickName,
                IsLocalPlayer = newPlayer.IsLocal
            };

            playersInRoom.Add(player);

            // Notify all clients about the new player
            photonView.RPC("NotifyPlayerJoined", RpcTarget.OthersBuffered, newPlayer.ActorNumber, newPlayer.NickName);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player left room: {otherPlayer.NickName} (ActorNumber: {otherPlayer.ActorNumber})");

            // Remove the player from our local list
            playersInRoom.RemoveAll(p => p.ActorNumber == otherPlayer.ActorNumber);

            // Notify all clients about the player leaving
            photonView.RPC("NotifyPlayerLeft", RpcTarget.All, otherPlayer.ActorNumber);
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Left room");
            playersInRoom.Clear();
        }

        [PunRPC]
        private void NotifyPlayerJoined(int actorNumber, string playerName)
        {
            PlayerData newPlayer = new PlayerData
            {
                ActorNumber = actorNumber,
                Nickname = playerName,
                IsLocalPlayer = false
            };

            playersInRoom.Add(newPlayer);
            Debug.Log($"RPC: Player joined - {playerName} (ActorNumber: {actorNumber})");
        }

        [PunRPC]
        private void NotifyPlayerLeft(int actorNumber)
        {
            playersInRoom.RemoveAll(p => p.ActorNumber == actorNumber);
            Debug.Log($"RPC: Player left - ActorNumber: {actorNumber}");
        }

        public List<PlayerData> GetPlayersInRoom()
        {
            return playersInRoom;
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        public void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        private class PlayerData
        {
            public int ActorNumber;
            public string Nickname;
            public bool IsLocalPlayer;
        }
    }
}


