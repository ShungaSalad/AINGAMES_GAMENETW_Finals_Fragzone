using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{

    [Header("Spawn Points")]
    public Transform[] Spawner;

    [Header("Player Settings")]
    public GameObject PlayerPrefab; // must be in Resources folder
    private int PlayerId;
    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            StartCoroutine(DelaySpawn());
        }
        else
        {
            Debug.LogWarning("Not connected to Photon or not in a room yet.");
        }
    }
    */

    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(0.2f); // wait a frame or two
        SpawnPlayer();
        Debug.Log("Spawned player: " + PlayerId);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined.");
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            StartCoroutine(DelaySpawn());
        }
        else
        {
            Debug.LogWarning("Not connected to Photon or not in a room yet.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // This is called on all clients *already in the room*
        // when a *new* player joins.
        Debug.Log(newPlayer.NickName + " has joined the room.");
        // You can use this to update UI lists, spawn objects, etc.
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (PlayerPrefab == null)
        {
            Debug.LogError("Player prefab is missing in inspector!");
            return;
        }

        PlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("PlayerID: " + PlayerId);


        Transform SpawnLocation; //this gets the position of location transform

        switch (PlayerId)
        {
            case 1: SpawnLocation = Spawner[0]; break;
            case 2: SpawnLocation = Spawner[1]; break;
            case 3: SpawnLocation = Spawner[2]; break;
            case 4: SpawnLocation = Spawner[3]; break;
            case 5: SpawnLocation = Spawner[4]; break;
            default:
                int randomIndex = Random.Range(0, Spawner.Length);
                SpawnLocation = Spawner[randomIndex];
                break;
        }
        // The prefab MUST be in a folder called "Resources"
        PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnLocation.position, SpawnLocation.rotation);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.RemoveBufferedRPCs();
        SceneManager.LoadScene("Lobby");
    }
}