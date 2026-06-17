using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 3.5f;       // Kecepatan lari musuh
    public float detectionRange = 6f;   // Jarak pandang musuh untuk deteksi player
    public float attackRange = 0.8f;     // Jarak dekat banget untuk bisa mukul player

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;

    void Start()
    {
        // Otomatis mencari objek bernama "Player_Baron" di dalam game
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
        rb.gravityScale = 0f; // Biar musuh ga jatuh ke bawah karena ini game top-down
        rb.freezeRotation = true; // Biar musuh ga muter-muter pas nabrak dinding
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        // 1. Hitung jarak antara musuh dan Baron
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // 2. LOGIKA AI: Jika Baron berada di dalam jarak deteksi
        if (distanceToPlayer <= detectionRange)
        {
            // Jika Baron sudah dekat banget, langsung serang/mukul
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            // Jika masih agak jauh tapi ketahuan, kejar Baron!
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            // Jika player ga kelihatan, bikin musuh diam (Nanti bisa ditambahin sistem patroli)
            rb.linearVelocity = Vector2.zero;
        }
    }

    void ChasePlayer()
    {
        // Hitung arah menuju posisi Baron
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        
        // Gerakkan Rigidbody musuh ke arah Baron
        rb.linearVelocity = direction * moveSpeed;

        // OPSIONAL: Bikin musuh nengok ke arah player yang dikejar
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void AttackPlayer()
    {
        // Hentikan pergerakan saat memukul
        rb.linearVelocity = Vector2.zero;

        if (playerHealth != null)
        {
            // Panggil fungsi TakeDamage yang udah kita buat di script PlayerHealth kemarin!
            playerHealth.TakeDamage();
        }
    }

    // Fungsi pembantu buat nampilin radius deteksi di Unity Editor (Biar lu kelihatan lingkarannya)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}