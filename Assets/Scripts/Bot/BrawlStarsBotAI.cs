using UnityEngine;
using UnityEngine.AI;

public class BrawlStarsBotAI : MonoBehaviour
{
    public float wanderRadius = 5f;  // ������ ���������
    public float wanderTimer = 3f;   // ������� ����� ����� ���������
    public float detectionRange = 10f; // ��������� ����������� ������
    public float attackRange = 3f;   // ��������� ����� (���� �����)

    private Transform player;        // ������ �� ������
    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // ���������, ��� � ������ ��� "Player"
        timer = wanderTimer;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� ����� � ���� �����������, ���� � ����
        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);

            // ���� ����� ��������� ��� �����������
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }
        // ����� ��������
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

    // ��������� ��������� ����� � NavMesh
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    // ����� ����� (����� ����������)
    private void AttackPlayer()
    {
        Debug.Log("������ ������!");
        // ����� ����� ���� �������, ���� � �. �.
    }
}