using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pong
{
    public class MyBallNetwork : NetworkBehaviour
    {
        public float speed = 30;
        public Rigidbody2D rigidbody2d;
        private MyPongNetworkManager networkManager;

        public override void OnStartServer()
        {
            base.OnStartServer();
            networkManager = (MyPongNetworkManager)NetworkManager.singleton;

            rigidbody2d.simulated = true;

            StartCoroutine(BallServingWithDelay(2f, 1));
        }

        float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
        {
            // ascii art:
            // ||  1 <- at the top of the racket
            // ||
            // ||  0 <- at the middle of the racket
            // ||
            // || -1 <- at the bottom of the racket
            return (ballPos.y - racketPos.y) / racketHeight;
        }

        [ServerCallback]
        void OnCollisionEnter2D(Collision2D col)
        {
            //Assign Score and Next Turn
            if (col.transform.GetComponent<MyScoreWall>())
            {
                MyScoreWall scoreWall = col.transform.GetComponent<MyScoreWall>();
                rigidbody2d.velocity = Vector2.zero;
                if (scoreWall.playerNumber == 1)
                {
                    StartCoroutine(BallServingWithDelay(2f, 2));
                    networkManager.ScorePoint(scoreWall.playerNumber);
                }
                if (scoreWall.playerNumber == 2)
                {
                    StartCoroutine(BallServingWithDelay(2f, 1));
                    networkManager.ScorePoint(scoreWall.playerNumber);
                }
            }

            //Calculate Bounce Dir
            if (col.transform.GetComponent<MyPlayer>())
            {
                float y = HitFactor(transform.position,
                                    col.transform.position,
                                    col.collider.bounds.size.y);

                float x = col.relativeVelocity.x > 0 ? 1 : -1;

                Vector2 dir = new Vector2(x, y).normalized;

                rigidbody2d.velocity = dir * speed;
            }
        }

        private void LeftBall()
        {
            rigidbody2d.velocity = -Vector2.right * speed;
        }

        private void RightBall()
        {
            rigidbody2d.velocity = Vector2.right * speed;
        }

        /// <summary>
        /// Need to change the start of ball
        /// </summary>
        /// <param name="delay">delay</param>
        /// <param name="PlayerServing">Player 1 or 2</param>
        /// <returns></returns>
        private IEnumerator BallServingWithDelay(float delay, int PlayerServing)
        {
            yield return new WaitForSeconds(delay);
            if (PlayerServing == 1)
                RightBall();
            if (PlayerServing == 2)
                LeftBall(); 
        }
    }
}