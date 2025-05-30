using UnityEngine;
using System.Collections;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _respawnTime = 3f;
    [SerializeField] private float _spawnYPosition = -17.65f; // Настраиваемая высота

    public void RespawnPlayer(GameObject player)
    {
        StartCoroutine(RespawnCoroutine(player));
    }

    private IEnumerator RespawnCoroutine(GameObject player)
    {
        player.SetActive(false);
        yield return new WaitForSeconds(_respawnTime);

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        Vector3 spawnPosition = new Vector3(
            spawnPoint.position.x,
            _spawnYPosition,
            spawnPoint.position.z
        );

        player.transform.position = spawnPosition;
        player.transform.rotation = spawnPoint.rotation;
        player.SetActive(true);

        player.GetComponent<Health>().ResetHealth();
    }

    // Для дебага в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var point in _spawnPoints)
        {
            Vector3 spawnPos = new Vector3(point.position.x, _spawnYPosition, point.position.z);
            Gizmos.DrawSphere(spawnPos, 0.5f);
            Gizmos.DrawLine(point.position, spawnPos);
        }
    }
}