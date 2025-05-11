using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject Player;
    public List<Transform> spawnPoints;
    public Joystick joystick; // ���������� ���� �������� �� Canvas

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        GameObject player = PhotonNetwork.Instantiate(Player.name, spawnPoints[randomIndex].position, spawnPoints[randomIndex].rotation);

        // ������� �������� ���������� ������
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<PlayerMovement>().joystick = joystick;
        }
    }
}