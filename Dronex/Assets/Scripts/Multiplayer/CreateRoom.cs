using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField maxPlayerSizeInput;

    [Header("Bools")]
    [SerializeField] public bool canCreateRoom;

    [Header("Integers")]
    [SerializeField] private int defaultValue = 7;
    // Start is called before the first frame update
    void Awake()
    {
        ConnectToMaster();
    }

    private void ConnectToMaster()
    {
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    // Create Room Function
    public void CreateRoomFunction()
    {
        if (!canCreateRoom)
        {
            return;
        }

        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? "Room" + UnityEngine.Random.Range(100, 90000) : roomNameInput.text;
        string roomPlayerSize = string.IsNullOrEmpty(maxPlayerSizeInput.text) ? defaultValue.ToString() : maxPlayerSizeInput.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = byte.Parse(roomPlayerSize);

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }
}
