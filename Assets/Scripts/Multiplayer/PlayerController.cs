







using UnityEngine;
using Photon.Pun;

namespace ChainEmpires
{
    public class PlayerController : MonoBehaviourPun, IPunObservable
    {
        [Header("Player Settings")]
        public int playerId = -1;
        public bool isMasterClient = false;
        public float moveSpeed = 5f;
        public float rotationSpeed = 3f;

        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private Vector3 velocity = Vector3.zero;

        [Header("Visuals")]
        public GameObject playerModel; // The visual representation
        public Material localPlayerMaterial;
        public Material remotePlayerMaterial;

        void Start()
        {
            if (photonView.IsMine)
            {
                Debug.Log($"Local player {playerId} initialized");
                SetupLocalPlayer();
            }
            else
            {
                Debug.Log($"Remote player {playerId} initialized");
                SetupRemotePlayer();
            }

            // Initialize network sync variables
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }

        public void Initialize(int id, bool isMaster)
        {
            playerId = id;
            isMasterClient = isMaster;

            if (photonView.IsMine)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupRemotePlayer();
            }
        }

        private void SetupLocalPlayer()
        {
            // Local player setup
            if (playerModel != null && localPlayerMaterial != null)
            {
                Renderer[] renderers = playerModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.material = localPlayerMaterial;
                }
            }

            Debug.Log($"Local player {playerId} setup complete");
        }

        private void SetupRemotePlayer()
        {
            // Remote player setup
            if (playerModel != null && remotePlayerMaterial != null)
            {
                Renderer[] renderers = playerModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.material = remotePlayerMaterial;
                }
            }

            Debug.Log($"Remote player {playerId} setup complete");
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            // Local player movement
            HandleMovement();
            HandleRotation();

            // Sync position to network
            transform.position = Vector3.SmoothDamp(transform.position, networkPosition, ref velocity, 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * rotationSpeed);
        }

        private void HandleMovement()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * moveSpeed * Time.deltaTime;

            if (movement != Vector3.zero)
            {
                transform.Translate(movement, Space.World);
            }
            #endif

            // Mobile touch controls would go here
        }

        private void HandleRotation()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            float rotate = Input.GetAxis("Rotate");

            if (rotate != 0f)
            {
                transform.Rotate(Vector3.up, rotate * rotationSpeed * Time.deltaTime);
            }
            #endif

            // Mobile swipe controls would go here
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send position and rotation to other players
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                // Receive position and rotation from other players
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();

                if (!photonView.IsMine)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, networkPosition, ref velocity, 0.1f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }

        public void SetPlayerName(string name)
        {
            if (photonView.IsMine && playerId != -1)
            {
                PhotonNetwork.NickName = name;
                photonView.RPC("UpdatePlayerName", RpcTarget.All, playerId, name);
            }
        }

        [PunRPC]
        private void UpdatePlayerName(int id, string newName)
        {
            Debug.Log($"Player {id} renamed to {newName}");
            // Update UI or other components if needed
        }

        public void RequestAction(string actionType, Vector3 position = default(Vector3))
        {
            if (photonView.IsMine && isMasterClient)
            {
                photonView.RPC("ExecuteServerAction", RpcTarget.MasterClient, playerId, actionType, position);
            }
        }

        [PunRPC]
        private void ExecuteServerAction(int playerId, string actionType, Vector3 position)
        {
            Debug.Log($"Master client executing action for player {playerId}: {actionType} at {position}");

            // Example: Handle different action types
            switch (actionType)
            {
                case "build":
                    // Handle building construction
                    break;
                case "attack":
                    // Handle attack command
                    break;
                case "move":
                    // Handle movement command
                    break;
            }
        }

        public void OnPlayerDisconnect()
        {
            if (!photonView.IsMine)
            {
                Debug.Log($"Remote player {playerId} disconnected");
                Destroy(gameObject);
            }
        }
    }
}



