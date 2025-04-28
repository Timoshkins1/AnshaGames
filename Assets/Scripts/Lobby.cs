using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro; // ���������� ������������ ��� ��� TMP 

public class Lobby : MonoBehaviourPunCallbacks
{
    public GameObject createInput;
    public GameObject joinInput;

    public void Createroom()
    {
        TMP_InputField createInputField = createInput.GetComponent<TMP_InputField>(); // �������� TMP InputField
        string roomName = createInputField.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom()
    {
        TMP_InputField joinInputField = joinInput.GetComponent<TMP_InputField>(); // �������� TMP InputField
        string roomName = joinInputField.text;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}