using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject Player; // Префаб игрока (с PhotonView)
    public List<Transform> spawnPoints; // Список точек спавна
    public GameObject Camera; // Префаб камеры игрока

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // Выбираем случайную точку спавна
        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Спавним игрока в выбранной точке
        GameObject player = PhotonNetwork.Instantiate(Player.name, spawnPoint.position, spawnPoint.rotation);

        // Спавним камеру и настраиваем ее следовать за игроком
        if (Camera != null)
        {
            // Находим существующую камеру (предполагается, что она уже есть на сцене)
            GameObject cameraHolder = GameObject.Find("Main Camera");
            CameraFollow cameraFollow = cameraHolder.GetComponent<CameraFollow>();

            if (cameraFollow != null)
            {
                cameraFollow.Player = player.transform; //Назначаем игрока для следования

            }
            else
            {
                Debug.LogError("Префаб камеры не имеет компонента CameraFollow!");
            }
        }
        else
        {
            Debug.LogError("Не назначен префаб камеры!");
        }
    }
}