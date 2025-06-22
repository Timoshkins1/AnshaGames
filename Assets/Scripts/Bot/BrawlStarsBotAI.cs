using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BrawlStarsBotAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkDetectionRange = 50f;
    public float runDetectionRange = 20f;
    public float attackRange = 2f;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float RunningSpeed = 1.0f;
    public float MovementSpeed = 1.0f;

    [Header("Attack Settings")]
    public float attack1Delay = 0.5f;
    public float attack2Delay = 0.8f;
    public int damage = 10;
    public float attackCooldown = 2f;

    [Header("References")]
    public Transform player;
    public Animator animator;
    public NavMeshAgent agent;
    public Collider hitCollider; // Коллайдер для атаки
    public Collider detectionCollider; // Отдельный коллайдер для обнаружения объектов

    private float timer;
    private float attackTimer;
    private bool isDead = false;
    private bool isAttacking = false;
    private int attackType = 0;
    private EnemyHealth enemyHealth;
    private Health playerHealth;
    private ObjectHealth currentDestroyableTarget;

    void Start()
    {
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            Debug.LogError("Bot starting position is not on NavMesh!");
            enabled = false;
            return;
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth == null)
            {
                Debug.LogWarning("Player doesn't have Health component!");
            }
        }

        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
        agent.autoBraking = false;
        agent.autoRepath = true;

        // Инициализация коллайдеров
        if (hitCollider != null)
            hitCollider.enabled = false;

        if (detectionCollider != null)
            detectionCollider.isTrigger = true; // Убедимся, что это триггер

        timer = wanderTimer;
        attackTimer = attackCooldown;

        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += HandleExternalDeath;
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= HandleExternalDeath;
        }
    }

    void Update()
    {
        if (isDead || !agent.enabled || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        // Проверяем наличие разрушаемых объектов в радиусе атаки
        ObjectHealth nearestDestroyable = FindNearestDestroyableInAttackRange();

        // Если нашли разрушаемый объект в радиусе атаки
        if (nearestDestroyable != null && !isAttacking && attackTimer <= 0)
        {
            currentDestroyableTarget = nearestDestroyable;
            StartAttack();
        }
        // Иначе проверяем игрока
        else if (distanceToPlayer <= attackRange && !isAttacking && attackTimer <= 0 && currentDestroyableTarget == null)
        {
            StartAttack();
        }
        else if (!isAttacking)
        {
            if (distanceToPlayer <= runDetectionRange)
            {
                ChasePlayer(true);
            }
            else if (distanceToPlayer <= walkDetectionRange)
            {
                ChasePlayer(false);
            }
            else
            {
                Wander();
            }
        }

        if (agent.velocity.magnitude < 0.1f)
        {
            animator.SetFloat("Speed", 0f);
        }
    }
    private ObjectHealth FindNearestDestroyableInAttackRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Destroyable"));
        ObjectHealth nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            ObjectHealth objectHealth = hitCollider.GetComponent<ObjectHealth>();
            if (objectHealth != null)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = objectHealth;
                }
            }
        }
        return nearest;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Обнаружение только через специальный коллайдер
        if (detectionCollider != null && other.gameObject.layer == LayerMask.NameToLayer("Destroyable"))
        {
            ObjectHealth objectHealth = other.GetComponent<ObjectHealth>();
            if (objectHealth != null)
            {
                currentDestroyableTarget = objectHealth;
                // Поворачиваемся к объекту
                transform.LookAt(new Vector3(objectHealth.transform.position.x, transform.position.y, objectHealth.transform.position.z));

                // Останавливаем бота и начинаем атаку
                agent.isStopped = true;
                if (!isAttacking && attackTimer <= 0)
                {
                    StartAttack();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectionCollider != null && other.gameObject.layer == LayerMask.NameToLayer("Destroyable"))
        {
            if (currentDestroyableTarget != null && currentDestroyableTarget.gameObject == other.gameObject)
            {
                currentDestroyableTarget = null;
                if (!isAttacking)
                {
                    agent.isStopped = false;
                }
            }
        }
    }
    private void Wander()
    {
        if (agent.remainingDistance < 0.5f || !agent.hasPath)
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                if (newPos != Vector3.zero)
                {
                    agent.SetDestination(newPos);
                    timer = 0;
                }
            }
        }

        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Running", false);
    }

    private void ChasePlayer(bool running)
    {
        if (agent.isOnNavMesh && agent.enabled)
        {
            agent.SetDestination(player.position);
            agent.speed = running ? RunningSpeed : MovementSpeed;

            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("Running", running);
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        attackType = Random.Range(1, 3);

        animator.ResetTrigger("Attack");
        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attack");

        if (attackType == 1)
            Invoke("DoDamage", attack1Delay);
        else
            Invoke("DoDamage", attack2Delay);

        Invoke("EndAttack", 2f);
    }

    private void DoDamage()
    {
        if (isDead) return;

        if (currentDestroyableTarget != null &&
            Vector3.Distance(transform.position, currentDestroyableTarget.transform.position) <= attackRange * 1.2f)
        {
            if (hitCollider != null)
            {
                hitCollider.enabled = true;
                Invoke("DisableHitCollider", 0.1f);
            }

            currentDestroyableTarget.TakeDamage(damage);
            Debug.Log($"Bot attacked destroyable object for {damage} damage!");

            // Сбрасываем цель сразу после удара
            currentDestroyableTarget = null;
        }
        else if (playerHealth != null && Vector3.Distance(transform.position, player.position) <= attackRange * 1.2f && HasLineOfSightToPlayer())
        {
            if (hitCollider != null)
            {
                hitCollider.enabled = true;
                Invoke("DisableHitCollider", 0.1f);
            }

            playerHealth.TakeDamage(damage);
            CameraFollow.ShakeCamera(0.6f, 0.2f);
            Debug.Log($"Bot attacked player for {damage} damage!");
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Vector3 direction = player.position - transform.position;

        if (Physics.Raycast(transform.position + Vector3.up, direction.normalized, out hit, attackRange * 1.2f))
        {
            return hit.collider.transform == player;
        }
        return false;
    }

    private void DisableHitCollider()
    {
        if (hitCollider != null)
            hitCollider.enabled = false;
    }

    private void EndAttack()
    {
        if (!isDead && agent != null)
        {
            isAttacking = false;
            agent.isStopped = false;
            attackTimer = attackCooldown;
            animator.ResetTrigger("Attack");
        }
    }

    public void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        int deathType = Random.Range(1, 3);
        animator.SetInteger("DeathType", deathType);
        animator.SetTrigger("Die");

        foreach (var collider in GetComponents<Collider>())
            collider.enabled = false;

        StartCoroutine(DestroyAfterAnimation(deathType));
    }

    private void HandleExternalDeath()
    {
        HandleDeath();
    }

    private IEnumerator DestroyAfterAnimation(int deathType)
    {
        yield return null;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        Destroy(gameObject);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, runDetectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkDetectionRange);
    }
}