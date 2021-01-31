using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NaturalSelection.Mirror.Game.Lobby
{
    public class NetworkGamePlayerLobby : NetworkBehaviour
    {
        [SyncVar]
        private string displayName = "Loading...";

        private NetworkManagerLobby room;

        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

            Room.GamePlayers.Add(this);
        }

        public void OnNetworkDestroy()
        {
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
    }

}
