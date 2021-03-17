﻿using Mirror;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NaturalSelection.Mirror.Game.Lobby
{
    public class NetworkManagerLobby : NetworkManager
    {
        [SerializeField] private int minPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

        [Header("Game")]
        [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
        [SerializeField] private GameObject playerSpawnSystem = null;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied;


        public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

        public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                ClientScene.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().path != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;

                NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

                roomPlayerInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if(conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            RoomPlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < minPlayers) { return false; }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady) { return false; }
            }

            return true;
        }

        public void StartGame()
        {
            if(SceneManager.GetActiveScene().path == menuScene)
            {
                if (!IsReadyToStart()) { return; }

                ServerChangeScene("newMap1");
            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            //From menu to game
            if(SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("newMap1"))
            {
                for(int i = RoomPlayers.Count - 1; i >=0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                    gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
                    NetworkServer.Destroy(conn.identity.gameObject);
                }
            }
            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName.StartsWith("newMap1"))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
            }
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);

            OnServerReadied?.Invoke(conn);
        }
    }
}

