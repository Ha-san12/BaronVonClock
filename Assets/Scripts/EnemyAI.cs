using UnityEditor.Animations;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 3.5f;       
    public float detectionRange = 6f;   
    public float attackRange = 0.8f;     

    [Header("Field of View & Line of Sight")]
    public bool useFOV = true;          
    public float fieldOfView = 90f;     
    public LayerMask obstacleLayer;     
    public bool showFOVVisuals = true;  

    [Header("Patrol Settings (Jalan-jalan Santai)")]
    public bool canPatrol = true;       
    public float patrolSpeed = 1.5f;    
    public float changeDirectionTime = 3f; 
    
    private float patrolTimer;
    private Vector2 patrolDirection;

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;

    private Animator anim;

    private bool hasTriggeredKaget = false;

    [Header("Audio Settings")]
    [SerializeField] private string namaSFXAttack;
    [SerializeField] private string namaSFXJalan;

    void Start()
    {
        GameObject player = GameObject.Find("Player_Baron");
        
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("Musuh tidak bisa menemukan objek 'Player_Baron'. Pastikan namanya sesuai di Hierarchy!");
        }

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
        rb.freezeRotation = true; 

        anim = GetComponent<Animator>();

        patrolTimer = changeDirectionTime;
        PickNewPatrolDirection();
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool canSeePlayer = false;

        // 1. Cek jangkauan
        if (distanceToPlayer <= detectionRange)
        {
            canSeePlayer = true;

            // 2. Cek sudut pandang (FOV)
            if (useFOV)
            {
                Vector2 enemyForward = -transform.up; 
                Vector2 dirToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
                float angleToPlayer = Vector2.Angle(enemyForward, dirToPlayer);

                if (angleToPlayer > fieldOfView / 2f)
                {
                    canSeePlayer = false; 
                }
            }

            // 3. Cek halangan tembok (Line of Sight)
            if (canSeePlayer)
            {
                RaycastHit2D hitInfo = Physics2D.Linecast(transform.position, playerTransform.position, obstacleLayer);
                if (hitInfo.collider != null)
                {
                    canSeePlayer = false; 
                }
            }
        }

        if (canSeePlayer)
        {
            patrolTimer = 0f; 

            if (!hasTriggeredKaget)
            {
                if (anim != null) anim.SetTrigger("isTriggered");
                hasTriggeredKaget = true;
            }

            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
        else 
        {
            if (hasTriggeredKaget)
            {
                hasTriggeredKaget = false;
            }

            if (canPatrol)
            {
                PatrolAround();
            }
            else
            {
                rb.linearVelocity = Vector2.zero; 
            }
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // if (anim != null) anim.SetBool("isAttacking", false);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;

        if (anim != null) anim.SetTrigger("isAttacking");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage();
        }
    }

    void PatrolAround()
    {
        patrolTimer -= Time.fixedDeltaTime;

        if (patrolTimer <= 0f)
        {
            PickNewPatrolDirection();
            patrolTimer = changeDirectionTime;
        }

        rb.linearVelocity = patrolDirection * patrolSpeed;

        if (patrolDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(patrolDirection.y, patrolDirection.x) * Mathf.Rad2Deg + 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }

        if (anim != null) anim.SetBool("isAttacking", false);
    }

    void PickNewPatrolDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        patrolDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
    }

    // --- BARU: Sistem Pantulan Tembok (Biar nggak nyangkut) ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Mengecek apakah objek yang ditabrak itu bagian dari Layer "Wall" (Obstacle Layer)
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            // Ambil arah permukaan tembok yang ditabrak
            Vector2 wallNormal = collision.contacts[0].normal;
            
            // Rumus "Reflect" bikin musuh putar balik arah menjauhi tembok (seperti pantulan cermin/biliar)
            patrolDirection = Vector2.Reflect(patrolDirection, wallNormal).normalized;
            
            // Reset timer biar dia nggak buru-buru ganti arah lagi
            patrolTimer = changeDirectionTime;
        }
    }

    void OnDrawGizmos()
    {
        

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (useFOV && showFOVVisuals)
        {
            Gizmos.color = Color.cyan;
            Vector3 forward = -transform.up;

            Vector3 leftBoundary = Quaternion.Euler(0, 0, fieldOfView / 2f) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -fieldOfView / 2f) * forward;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange);
        }
    }
}