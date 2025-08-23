








using UnityEngine;
using Photon.Pun;

namespace ChainEmpires
{
    public class TestMultiplayerSetup : MonoBehaviourPunCallbacks
    {
        [Header("Test Setup")]
        public GameObject testPlayerPrefab;
        public Transform[] spawnPoints;

        void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log("Test multiplayer setup initialized");

                // Test player instantiation
                if (spawnPoints.Length > 0 && testPlayerPrefab != null)
                {
                    Vector3 spawnPosition = spawnPoints[0].position;
                    Quaternion spawnRotation = spawnPoints[0].rotation;

                    GameObject playerObj = PhotonNetwork.Instantiate(testPlayerPrefab.name, spawnPosition, spawnRotation);
                    PlayerController playerController = playerObj.GetComponent<PlayerController>();

                    if (playerController != null)
                    {
                        playerController.Initialize(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.IsMasterClient);
                    }
                }

                // Test realm convergence
                RealmConvergence realmSystem = FindObjectOfType<RealmConvergence>();
                if (realmSystem != null)
                {
                    realmSystem.AddPlayerToRealm(PhotonNetwork.LocalPlayer.ActorNumber, 1); // Add to Stone Age realm
                }
            }
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("Test: Player left room");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log($"Test: Disconnected from Photon - Reason: {cause}");
        }
    }
}




