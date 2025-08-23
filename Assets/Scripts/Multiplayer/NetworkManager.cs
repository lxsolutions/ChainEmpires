using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic; // For matchmaking queue
using ExitGames.Client.Photon; // For custom properties

namespace ChainEmpires
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [Header("Photon Settings")]
        [SerializeField] private string appVersion = "1.0"; // For matchmaking pools
        [SerializeField] private byte maxPlayersPerRoom = 100; // Realm cap

        [Header("Fairness & Innovations")]
        [SerializeField] private bool useEloMatchmaking = true; // Pair similar progression
        [SerializeField] private float eloTolerance = 200f; // Match range
        [SerializeField] private float portalDebuff = 0.2f; // Advanced player stat reduction in lower realms

        private Dictionary<int, RoomInfo> realmRooms = new(); // Sharding by realm ID
        private Queue<Player> matchmakingQueue = new(); // For Elo

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            ConnectToPhoton();
        }

        private void ConnectToPhoton()
        {
            PhotonNetwork.GameVersion = appVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master");
            JoinLobby();
        }

        private void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby");
            // Innovative Realm Join: Based on player progression
            int playerRealm = ProgressionManager.Instance.CurrentRealmIndex;
            JoinOrCreateRealmRoom(playerRealm);
        }

        private void JoinOrCreateRealmRoom(int realmId)
        {
            string roomName = $"Realm_{realmId}";
            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayersPerRoom };
            options.CustomRoomProperties = new Hashtable { { "realmId", realmId } };
            options.CustomRoomPropertiesForLobby = new string[] { "realmId" };

            PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
            // Sync base state offline (Firebase fallback if disconnected)
            if (!PhotonNetwork.IsConnected) SyncOffline();
            // Portal Event: If cross-realm, apply debuff
            if (IsCrossRealm()) ApplyDebuff(portalDebuff);
        }

        private bool IsCrossRealm()
        {
            int roomRealm = (int)PhotonNetwork.CurrentRoom.CustomProperties["realmId"];
            return roomRealm != ProgressionManager.Instance.CurrentRealmIndex;
        }

        private void ApplyDebuff(float amount)
        {
            // Reduce stats (e.g., via UnitManager)
            Debug.Log($"Applied cross-realm debuff: -{amount * 100}% stats");
        }

        // Elo Matchmaking for PvP
        public void QueueForPvP(int playerElo)
        {
            matchmakingQueue.Enqueue(PhotonNetwork.LocalPlayer);
            Matchmake();
        }

        private void Matchmake()
        {
            while (matchmakingQueue.Count >= 2)
            {
                Player p1 = matchmakingQueue.Dequeue();
                Player p2 = matchmakingQueue.Dequeue();
                if (Mathf.Abs((int)p1.CustomProperties["elo"] - (int)p2.CustomProperties["elo"]) <= eloTolerance)
                {
                    // Create PvP room
                    string pvpRoom = $"PvP_{p1.UserId}_{p2.UserId}";
                    PhotonNetwork.CreateRoom(pvpRoom, new RoomOptions { MaxPlayers = 2 });
                }
                else matchmakingQueue.Enqueue(p1); // Requeue if no match
            }
        }

        // RPC Example: Async Attack
        [PunRPC]
        private void RpcAttack(string targetId)
        {
            // Resolve turn-based attack
            Debug.Log($"Received attack on {targetId}");
        }

        public void SendAttack(string targetId)
        {
            photonView.RPC("RpcAttack", PhotonTargets.All, targetId);
        }

        // Offline Sim: Use Firebase for persistence
        private void SyncOffline()
        {
            // Placeholder: Firebase.Database.Push(baseState)
            Debug.Log("Synced offline progress with Firebase");
        }

        // Opt: Low-bandwidth (compress data before sync)
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var room in roomList) realmRooms[(int)room.CustomProperties["realmId"]] = room;
        }
    }
}