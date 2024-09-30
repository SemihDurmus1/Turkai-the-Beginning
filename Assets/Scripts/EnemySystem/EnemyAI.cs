using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : EnemyBase
{
    [Header("AI Settings")]
    public float detectionRange = 10f;  // The maximum chase distance
    public float attackDistance = 2f;   // Attack Range

    [SerializeField] private float stopDistance = 1f;  // Enemy stand range of on the player
    [SerializeField] private Transform[] patrolPoints;    // Patrol points

    private int currentPatrolIndex = 0;
    protected NavMeshAgent navMeshAgent;
    public GameObject player;

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;

    protected override void Start()
    {
        base.Start();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        GoToNextPatrolPoint();
        //enemyAnimator.applyRootMotion = false;
    }

    private void Update()
    {
        if (isDead) { return; }

        if (canMove == false) { navMeshAgent.speed = 0; return; }
        else { navMeshAgent.speed = walkSpeed; }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRange)//range between player, if less then detectRange
        {
            ChasePlayer();
        }
        //If player arent in range
        else if (!navMeshAgent.pathPending/*If path is null*/ && navMeshAgent.remainingDistance < 0.5f)//current point closer than 0.5f
        {
            detectionRange = 10f;
            GoToNextPatrolPoint(); //Voltaya devam babba
        }

        // Check player in attackRange
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            enemyAnimator.SetBool("isChasing", false);
            StartAttackAnim();
            enemyAnimator.SetBool("isAttacking", true);
        }
        else
        {
            enemyAnimator.SetBool("isAttacking", false);
        }
    }

    private void ChasePlayer()
    {
        navMeshAgent.speed = sprintSpeed;
        detectionRange = 20f;

        // Player'a olan yönü hesapla (normalleþtirildiðinde mesafe baðýmsýz hale gelir)
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // Yeni hedef pozisyon: Player'dan "stopDistance" kadar uzakta olacak þekilde ayarla
        Vector3 targetPosition = player.transform.position - directionToPlayer * stopDistance;

        // Hedef pozisyona git
        navMeshAgent.SetDestination(targetPosition);

        // Make sure the enemy rotates smoothly towards the player
        if (navMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            // Rotate the enemy smoothly towards the movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(navMeshAgent.velocity.normalized), Time.deltaTime * 10f);
        }

        enemyAnimator.SetBool("isChasing", true);
        enemyAnimator.SetBool("isAttacking", false);
        canMove = true;
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        navMeshAgent.speed = walkSpeed;
        navMeshAgent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        enemyAnimator.SetBool("isChasing", false);
        enemyAnimator.SetBool("isAttacking", false);
        canMove = true;
    }
}
