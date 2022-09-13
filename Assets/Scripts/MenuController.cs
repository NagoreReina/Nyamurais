using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;


public class MenuController : MonoBehaviour
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private InputField UsernameInput;
    [SerializeField] private GameObject StartButton;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    private void Start()
    {

    }
    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); //ANTERIOR TUTORIAL
        PhotonNetwork.automaticallySyncScene = true;
        Debug.Log("Connected");
    }

    public void ChangeUserNameInput()
    {
        if (UsernameInput.text.Length >= 3 && UsernameInput.text.Length <= 10)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
    }
    public void JoinGame()
    {
        PhotonNetwork.playerName = UsernameInput.text;
        PhotonNetwork.JoinOrCreateRoom("DEFAUT", new RoomOptions() { MaxPlayers = 20 }, TypedLobby.Default);        
    }
    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("WaitingScene");
    }

}
