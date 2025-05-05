using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject Player; // ������ ������ (� PhotonView)
    public List<Transform> spawnPoints; // ������ ����� ������
    public GameObject Camera; // ������ ������ ������

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // �������� ��������� ����� ������
        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[randomIndex];

        // ������� ������ � ��������� �����
        GameObject player = PhotonNetwork.Instantiate(Player.name, spawnPoint.position, spawnPoint.rotation);

        // ������� ������ � ����������� �� ��������� �� �������
        if (Camera != null)
        {
            // ������� ������������ ������ (��������������, ��� ��� ��� ���� �� �����)
            GameObject cameraHolder = GameObject.Find("Main Camera");
            CameraFollow cameraFollow = cameraHolder.GetComponent<CameraFollow>();

            if (cameraFollow != null)
            {
                cameraFollow.Player = player.transform; //��������� ������ ��� ����������

            }
            else
            {
                Debug.LogError("������ ������ �� ����� ���������� CameraFollow!");
            }
        }
        else
        {
            Debug.LogError("�� �������� ������ ������!");
        }
    }
}