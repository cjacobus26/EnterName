using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public GameObject lobbyUI, waitingUI;
    public TMP_InputField createInput, joinInput;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text.ToLower());
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text.ToLower());
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("WaitingForPlayers");
    }
}
