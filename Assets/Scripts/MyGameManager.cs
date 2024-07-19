using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Pong
{
    public class MyGameManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnP1ScoreChanged))] 
        private int p1Score = 0;
        [SyncVar(hook = nameof(OnP2ScoreChanged))]
        private int p2Score = 0;

        public TextMeshPro player1Score;
        public TextMeshPro player2Score;
        public TextMeshPro WaitText;
        public TextMeshPro WinP1Text;
        public TextMeshPro WinP2Text;

        private const int winScore = 11;

        public void InitializeGame()
        {
            RpcToggleUI(false, false, false, false);
            p1Score = 0;
            p2Score = 0;
            UpdateScoreUI();
        }

        public void StartGame()
        {
            p1Score = 0;
            p2Score = 0;
            UpdateScoreUI();
            RpcToggleUI(true, false, false, false);
        }

        public void WaitPlayer()
        {
            RpcToggleUI(false, true, false, false);
        }

        [Server]
        public void IncrementScore(int playerNumber)
        {
            if (playerNumber == 1)
            {
                p1Score++;
            }
            else if (playerNumber == 2)
            {
                p2Score++;
            }
            UpdateScoreUI();
            CheckForWin();
        }

        [Server]
        private void CheckForWin()
        {
            if (p1Score == 11)
            {
                RpcShowWinText(1);
            }
            else if (p2Score == 11)
            {
                RpcShowWinText(2);
            }
        }

        [Server]
        public bool HasPlayerWon()
        {
            return p1Score >= winScore || p2Score >= winScore;
        }


        [ClientRpc]
        private void RpcShowWinText(int playerNumber)
        {
            if (playerNumber == 1) WinP1Text.gameObject.SetActive(true);
            else WinP2Text.gameObject.SetActive(true);
        }

        private void OnP1ScoreChanged(int oldScore, int newScore)
        {
            UpdateScoreUI();
        }

        private void OnP2ScoreChanged(int oldScore, int newScore)
        {
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            player1Score.text = p1Score.ToString();
            player2Score.text = p2Score.ToString();
        }

        [ClientRpc]
        private void RpcToggleUI(bool showScores, bool showWait, bool showWinP1, bool showWinP2)
        {
            player1Score.gameObject.SetActive(showScores);
            player2Score.gameObject.SetActive(showScores);
            WaitText.gameObject.SetActive(showWait);
            WinP1Text.gameObject.SetActive(showWinP1);
            WinP2Text.gameObject.SetActive(showWinP2);
        }
    }
}
