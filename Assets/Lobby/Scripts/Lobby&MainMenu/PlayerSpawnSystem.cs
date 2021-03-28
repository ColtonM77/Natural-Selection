using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NaturalSelection.Mirror.Game.Lobby
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        //[SerializeField] private GameObject playerPrefab = null;
        [SerializeField] private GameObject player1 = null;
        [SerializeField] private GameObject player2 = null;
        [SerializeField] private GameObject player3 = null;
        [SerializeField] private GameObject player4 = null;

        public static List<GameObject> characterList = new List<GameObject>();

        private static List<Transform> spawnPoints = new List<Transform>();

        private int nextIndex = 0;

        public void Awake()
        {
            characterList.Add(player1);
            characterList.Add(player2);
            characterList.Add(player3);
            characterList.Add(player4);
        }
        public static void AddSpawnPoint(Transform transform)
        {
            spawnPoints.Add(transform);

            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

        public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

        [ServerCallback]
        private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

        [Server]
        public void SpawnPlayer(NetworkConnection conn)
        {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
            if (spawnPoint == null)
            {
                Debug.LogError($"Missing spawn point for player {nextIndex}");
                return;
            }

            GameObject playerInstance = Instantiate(characterList[nextIndex], spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
            NetworkServer.AddPlayerForConnection(conn, playerInstance);

            nextIndex++;
        }
    }
}
