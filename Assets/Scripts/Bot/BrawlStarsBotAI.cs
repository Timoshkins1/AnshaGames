using UnityEngine;
using UnityEngine.AI;

public class BrawlStarsBotAI : MonoBehaviour
{
    public float wanderRadius = 5f;  // Радиус блуждания
    public float wanderTimer = 3f;   // Частота смены точки блуждания
    public float detectionRange = 10f; // Дистанция обнаружения игрока
    public float attackRange = 3f;   // Дистанция атаки (если нужно)

    private Transform player;        // Ссылка на игрока
    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Убедитесь, что у игрока тег "Player"
        timer = wanderTimer;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Если игрок в зоне обнаружения, идем к нему
        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);

            // Если нужно атаковать при приближении
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }
        // Иначе блуждаем
        else
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    // Генерация случайной точки в NavMesh
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    // Метод атаки (можно доработать)
    private void AttackPlayer()
    {
        Debug.Log("Атакую игрока!");
        // Здесь может быть выстрел, удар и т. д.
    }
}