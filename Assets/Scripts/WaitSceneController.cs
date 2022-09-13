using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitSceneController : MonoBehaviour
{
    public TextMeshProUGUI txtPlayerName, txtWaiteTimmer, txtNumOfPlayer, txtWaitingForPlayers;
    public float waitTimmer;
    private PhotonView myPhotonView;
    void Start()
    {
        txtPlayerName.text = "You are: " + PhotonNetwork.playerName;
        waitTimmer = 30f;
        myPhotonView = GetComponent<PhotonView>();
        PhotonNetwork.automaticallySyncScene = true;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        txtNumOfPlayer.text = "Connected Players: " + PhotonNetwork.playerList.Length + " /20";


        if (PhotonNetwork.playerList.Length < 1)
        {
            waitTimmer = 30f;
            txtWaiteTimmer.text = "Not enough players to start";
        }
        else if (PhotonNetwork.playerList.Length >= 1 && waitTimmer > 0)
        {
            waitTimmer -= Time.deltaTime;
            txtWaiteTimmer.text = "Time to start: " + waitTimmer.ToString("F0");
        }
        else if(waitTimmer <= 0)
        {
            txtWaiteTimmer.text = "Loading game...";
        }
        if (PhotonNetwork.playerList.Length >= 5 && waitTimmer >= 5)
        {
            waitTimmer = 4;
            
        }
        else
        {
            txtWaitingForPlayers.text = "Waiting for players...";
        }
        if (waitTimmer <= 0)
        {
            OnWaitComplete();
        }
        if (waitTimmer <= 5)
        {
            txtWaitingForPlayers.text = "Almost ready!...";
        }
    }
    private void OnWaitComplete()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.LoadLevel("Game");
    }
    public void OnPhotonPlayerConnected (PhotonPlayer player)
    {
        waitTimmer = 20;
    }
}
