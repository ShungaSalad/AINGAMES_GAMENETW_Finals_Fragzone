using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonLoby : MonoBehaviourPunCallbacks
{
    [Header("Photon Connection")]
    public static PhotonLoby lobby; //this is referenced to the photon lobby (ignore the class wrong spelling)
    public string sceneToLoad;

    [Header("Object References")]
    public GameObject createButton;
    public GameObject joinButton;
    public GameObject cancelButton;
    public GameObject offlineButton;

    [Header("UI Input References")]
    public TMP_InputField inputID;
    public TMP_InputField joinID;

    [Header("UI Message References")]
    public TMP_Text sysmsg;
    const string CreateMSG = "Creating a room...";
    const string JoinMSG = "Joining a room...";
    const string ErrorMSGA = "Why not you create that room while it doesn't exist yet?";
    const string ErrorMSGB = "It's too quiet. Please try creating a room.";
    const string ErrorMSGC = "Isn't that room already created? Please try joining it.";

    private void Awake()
    {
        lobby = this;
        PhotonNetwork.AutomaticallySyncScene = true;
        sysmsg.text = "Welcome to FragZone!";
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //connects to the photon server
        offlineButton.gameObject.SetActive(true); //displays offline if we are not connected to the server
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon Master server");
        //PhotonNetwork.AutomaticallySyncScene = true;
        LobbyMenuOnlineState();
        offlineButton.SetActive(false); //this will be disabled if we are connected to the server
    }

    private void LobbyMenuOnlineState()
    {
        createButton.SetActive(true); //this enables the start button allowing players to create a network  lobby
        joinButton.SetActive(true);//this enables the join button allowing players to join a network  lobby
        cancelButton.SetActive(false);
    }

    //button script for connecting to a network game
    public void BtnCreateRoom()
    {
        Debug.Log("Play Button was clicked");
        Debug.Log("Creating a room...");
        sysmsg.text = CreateMSG;
        CreateRoom();
        LobbyMenuProcessState();
    }

    private void LobbyMenuProcessState()
    {
        createButton.SetActive(false);
        joinButton.SetActive(false);
        cancelButton.SetActive(true);
    }

    //button script for joining a network game
    public void BtnJoinRoom()
    {
        Debug.Log("Joining a room...");
        JoinRoomById();
        LobbyMenuProcessState();
    }



    //button script for canceling connection to a network game
    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel Button Clicked");
        PhotonNetwork.LeaveRoom();
        LobbyMenuOnlineState();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random game but failed. There must be no open room");
        sysmsg.text = ErrorMSGB + "\n(" + message + ")";
        LobbyMenuOnlineState();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("How about you create that non-existing room?");
        sysmsg.text = ErrorMSGA + "\n(" + message + ")";
        LobbyMenuOnlineState();
    }

    void CreateRoom()
    {
        string roomName = inputID.text;
        if (string.IsNullOrEmpty(roomName)) //creates a random room if no input data detected
        {
            roomName = ""+Random.Range(1000, 9999); // fallback if empty
        }

        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 5
        };
        sysmsg.text = CreateMSG+"Room "+roomName+" (Max "+roomOps.MaxPlayers+" Players)";
        PhotonNetwork.CreateRoom(roomName, roomOps);
        Debug.Log($"Trying to create room: {roomName}");
    }

    void JoinRoomById()
    {
        string roomToJoin = joinID.text.Trim();

        if (string.IsNullOrEmpty(roomToJoin)) //joins a  random room if no input data detected
        {
            Debug.Log("No room name entered → joining a random open room...");
            sysmsg.text = JoinMSG + " (Quick Join)";
            PhotonNetwork.JoinRandomRoom(); // will trigger OnJoinRandomFailed if none exists
        }
        else //this allows us to connect to the specified room
        {
            sysmsg.text = JoinMSG + "Room "+roomToJoin;
            Debug.Log($"Trying to join specific room: {roomToJoin}");
            PhotonNetwork.JoinRoom(roomToJoin);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("We are now in a room");
        PhotonNetwork.LoadLevel(sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must be a room with the same name");
        sysmsg.text = ErrorMSGC + "\n(" + message + ")";
        LobbyMenuOnlineState();
    }



}
