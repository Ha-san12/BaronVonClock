using UnityEngine;

public class PolishedRangedEnemy : MonoBehaviour
{
    // --- STATE MACHINE ---
    public enum EnemyState { Patrol, Chase, Attack }
    [Header("Current State")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("References")]
    public Transform shootPoint;          
    public GameObject projectilePrefab;   

    [Header("Movement & Ranges")]
    public float moveSpeed = 3.5f;       //
    public float patrolSpeed = 1.5f;     //
    public float detectionRange = 6f;    //
    public float attackRange = 5f;

    [Header("Field of View & Line of Sight")]
    public bool useFOV = true;           //
    public float fieldOfView = 90f;      //[cite: 3]
    public LayerMask obstacleLayer;      //[cite: 3]
    public bool showFOVVisuals = true;   //[cite: 3]

    [Header("Combat Settings")]
    public float fireRate = 1.5f;
    public float projectileSpeed = 12f;
    public float rotationSpeed = 10f;     

    [Header("Patrol Settings")]
    public float changeDirectionTime = 3f; //[cite: 3]
    private float patrolTimer;           
    private Vector2 patrolDirection;     

    // Internal Variables
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator anim;
    private float nextFireTime;
    private bool hasTriggeredKaget = false; //[cite: 3]
    

    void Start()
    {
        GameObject player = GameObject.Find("Player_Baron"); //[cite: 3]
        if (player != null) playerTransform = player.transform;
        
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;             //[cite: 3]
        rb.freezeRotation = true;         //[cite: 3]
        
        anim = GetComponent<Animator>();

        patrolTimer = changeDirectionTime;
        PickNewPatrolDirection();        
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        CheckDetectionAndState();
        UpdateBehavior();
    }

    // --- 1. LOGIKA DETEKSI (FOV & LoS) & PENENTUAN STATUS ---
    void CheckDetectionAndState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool canSeePlayer = false; //[cite: 3]

        // 1. Cek jangkauan[cite: 3]
        if (distanceToPlayer <= detectionRange)
        {
            canSeePlayer = true;

            // 2. Cek sudut pandang (FOV)[cite: 3]
            if (useFOV)
            {
                Vector2 enemyForward = -transform.up; //[cite: 3]
                Vector2 dirToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
                float angleToPlayer = Vector2.Angle(enemyForward, dirToPlayer); //[cite: 3]

                if (angleToPlayer > fieldOfView / 2f) //[cite: 3]
                {
                    canSeePlayer = false; 
                }
            }

            // 3. Cek halangan tembok (Line of Sight)[cite: 3]
            if (canSeePlayer)
            {
                RaycastHit2D hitInfo = Physics2D.Linecast(transform.position, playerTransform.position, obstacleLayer); //[cite: 3]
                if (hitInfo.collider != null) //[cite: 3]
                {
                    canSeePlayer = false; 
                }
            }
        }

        // Set State berdasarkan hasil deteksi
        if (canSeePlayer)
        {
            patrolTimer = 0f; //[cite: 3]

            // Trigger animasi kaget jika baru pertama kali melihat player[cite: 3]
            if (!hasTriggeredKaget)
            {
                if (anim != null) anim.SetTrigger("isTriggered"); //[cite: 3]
                hasTriggeredKaget = true; //[cite: 3]
            }

            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
            }
            else
            {
                currentState = EnemyState.Chase;
            }
        }
        else 
        {
            // Reset kaget jika kehilangan jejak player[cite: 3]
            if (hasTriggeredKaget)
            {
                hasTriggeredKaget = false; //[cite: 3]
            }

            currentState = EnemyState.Patrol;
        }
    }

    // --- 2. LOGIKA PERILAKU SESUAI STATUS ---
    void UpdateBehavior()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolBehavior();
                break;
            
            case EnemyState.Chase:
                ChaseBehavior();
                break;
            
            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
    }

    // --- 3. DETAIL BEHAVIOR ---
    void PatrolBehavior()
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
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    void ChaseBehavior()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed);
    }

    void AttackBehavior()
    {
        rb.linearVelocity = Vector2.zero;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed);

        if (Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (anim != null) anim.SetTrigger("isAttacking");

        if (shootPoint != null && projectilePrefab != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null) 
            {
                Vector2 exactDirection = (playerTransform.position - shootPoint.position).normalized;
                bulletRb.linearVelocity = exactDirection * projectileSpeed;
            }
        }

        nextFireTime = Time.time + fireRate;
    }

    // --- 4. FUNGSI PENDUKUNG ---
    void PickNewPatrolDirection()
    {
        float randomAngle = Random.Range(0f, 360f); //[cite: 3]
        patrolDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized; //[cite: 3]
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0) //[cite: 3]
        {
            Vector2 wallNormal = collision.contacts[0].normal; //[cite: 3]
            patrolDirection = Vector2.Reflect(patrolDirection, wallNormal).normalized; //[cite: 3]
            patrolTimer = changeDirectionTime; //[cite: 3]
        }
    }

    // Gambar Visual FOV dan Jarak di Unity Editor[cite: 3]
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange); //[cite: 3]
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); //[cite: 3]

        if (useFOV && showFOVVisuals) //[cite: 3]
        {
            Gizmos.color = Color.cyan; //[cite: 3]
            Vector3 forward = -transform.up; //[cite: 3]

            Vector3 leftBoundary = Quaternion.Euler(0, 0, fieldOfView / 2f) * forward; //[cite: 3]
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -fieldOfView / 2f) * forward; //[cite: 3]

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange); //[cite: 3]
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange); //[cite: 3]
        }
    }
}