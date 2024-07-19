using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pong
{
    public class MyPongNetworkManager : NetworkManager
    {
        public Transform player1;
        public Transform player2;
        GameObject ball;

        public MyGameManager gameManagerInstance;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform start = numPlayers == 0 ? player1 : player2;
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);
            gameManagerInstance.WaitPlayer();
            if (numPlayers == 2)
            {
                gameManagerInstance.StartGame();
                SpawnBall();
            }
        }

        public override void OnClientDisconnect()
        {
            gameManagerInstance.WaitPlayer();
            base.OnClientDisconnect();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (ball != null)
                NetworkServer.Destroy(ball);

            base.OnServerDisconnect(conn);
            gameManagerInstance.InitializeGame();
        }

        //Spawn Ball always directioning for player 2
        private void SpawnBall()
        {
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            ball.transform.position = new Vector3(2, 0, 0);
            NetworkServer.Spawn(ball);
        }

        [Server]
        //Always change spawn y, the x is for player serving
        public void ResetBall(int xSpacing)
        {
            ball.transform.position = new Vector3(xSpacing, Random.Range(12, -12), 0);
        }

        [Server]
        //Add Score on server and prepare for next move
        public void ScorePoint(int playerNumber)
        {
            if (playerNumber == 1)
            {
                gameManagerInstance.IncrementScore(2);
                ResetBall(-2);
            }
            else if (playerNumber == 2)
            {
                gameManagerInstance.IncrementScore(1);
                ResetBall(2);
            }

            if (gameManagerInstance.HasPlayerWon())
            {
                NetworkServer.Destroy(ball);
            }
        }
    }
}