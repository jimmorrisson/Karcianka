using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.Polygon.CardGame
{
    public enum Hand
    {
        None,
        Card,
        Spell
    }

    public enum ResultType
    {
        None,
        Draw,
        LocalWin,
        LocalLoss
    }

    public class GameManager : Photon.PunBehaviour, IPunTurnManagerCallbacks
    {
        #region private variables
        private PunTurnManager turnManager;
        private GameManager GameManagerInstance { get; set; }
        [SerializeField]
        private float TurnDuration;
        #endregion

        #region public variables
        public Text TimeText;
        public Text PlayerText;
        public Text RemotePlayerText;
        public GameObject playerPrefab;
        #endregion

        [SerializeField]
        private Image localSelectionImage;
        public Hand localSelection;

        [SerializeField]
        private Image remoteSelectionImage;
        public Hand remoteSelection;

        private void Awake()
        {
            GameManagerInstance = this;
        }

        private void Start()
        {
            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
            this.turnManager.TurnManagerListener = this;
            this.turnManager.TurnDuration = TurnDuration;

            PlayerText.text = PhotonNetwork.player.NickName;

            Debug.Log("ID" + PhotonNetwork.player.ID);

            if(playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    if(PhotonNetwork.playerList.Length == 1)
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    else
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.Log("Ignoring scene load for " + Application.loadedLevelName);
                }
            }
        }

        private void Update()
        {
            this.TimeText.text = "Time: " + string.Format("{0:0.00}", this.turnManager.RemainingSecondsInTurn);
            UpdateTimeText();
        }

        public void StartTurn()
        {
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("Begin Turn");
                this.turnManager.BeginTurn();
            }
        }

        private void UpdateTimeText()
        {
            PhotonPlayer remote = PhotonNetwork.player.GetNext();
            PhotonPlayer local = PhotonNetwork.player;

            if (remote != null)
            {
                RemotePlayerText.text = remote.NickName;
            }
            else
            {
                RemotePlayerText.text = "waiting for another player...";
            }
        }

        //private void LoadArena()
        //{
        //    if (!PhotonNetwork.isMasterClient)
        //    {
        //        Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        //    }
        //    Debug.Log("Load main scene");
        //    PhotonNetwork.LoadLevel("Main");
        //}
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #region Photon Messages


        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("Other player arrived " + other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.room.PlayerCount == 2)
            {
                Debug.Log("Starting turn...");
                this.StartTurn();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            }
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.room.PlayerCount == 2)
            {
                if (this.turnManager.Turn == 0)
                {
                    Debug.Log("There are 2 players. Starting the game");
                    // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                    this.StartTurn();
                }
            }
            else
            {
                Debug.Log("Waiting for another player");
            }
        }

        public void OnTurnBegins(int turn)
        {
            Debug.Log("On turn " + turn);
            this.localSelection = Hand.None;
            this.remoteSelection = Hand.None;

        }

        public void OnTurnCompleted(int turn)
        {
            OnEndTurn();
        }

        public void OnPlayerMove(PhotonPlayer player, int turn, object move)
        {

        }

        public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
        {
            Debug.Log("OnTurnFinished: " + player + " turn: " + turn + " action: " + move);

            if (player.IsLocal)
            {
                this.localSelection = (Hand)(byte)move;
            }
            else
            {
                this.remoteSelection = (Hand)(byte)move;
            }
        }

        public void OnTurnTimeEnds(int turn)
        {
            OnTurnCompleted(-1);
        }


        #endregion
        public void OnEndTurn()
        {
            this.StartTurn();
        }
    }
}