using UnityEngine;
using System.Collections; 

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float maxSharpness = 100f;
    public float currentSharpness = 100f;
    public float sharpnessLossPerHit = 20f; 
    public int sharpenOrbCost = 2;          

    [Header("Attack Settings")]
    public float attackRange = 1.2f;        
    public Transform attackPoint;           
    public LayerMask enemyLayer;            

    [Header("Inventory Data")]
    public int currentMemoryOrbs = 5;       

    [Header("Sharpening Settings")]
    public float sharpenDuration = 4f;      
    private bool isSharpening = false;      
    private PlayerMovement playerMovement;  

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (isSharpening) return;

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TrySharpenWeapon();
        }
    }

    void Attack()
    {
        if (currentSharpness <= 0f)
        {
            Debug.Log("Senjata Tumpul! Damage 0. Tekan 'R' untuk menajamkan kembali!");
            return;
        }

        Debug.Log("Karakter mengayunkan pedang!");

        // 1. Deteksi dulu apakah ada musuh di dalam area serangan
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // 2. KONDISI BARU: Kurangi ketajaman HANYA jika array hitEnemies ada isinya (mengenai musuh)
        if (hitEnemies.Length > 0)
        {
            currentSharpness -= sharpnessLossPerHit;
            currentSharpness = Mathf.Clamp(currentSharpness, 0f, maxSharpness);
            Debug.Log("Tebasan mengenai target! Ketajaman Senjata Sisa: " + currentSharpness + "%");
        }
        else
        {
            Debug.Log("Ayunan meleset ke udara. Ketajaman tetap aman!");
        }

        // 3. Hancurkan musuh yang terkena tebasan
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Musuh hancur terkena serangan!");
            Destroy(enemy.gameObject); 
        }
    }

    void TrySharpenWeapon()
    {
        if (currentSharpness >= maxSharpness)
        {
            Debug.Log("Senjata masih tajam, tidak perlu di-sharpen!");
            return;
        }

        if (currentMemoryOrbs >= sharpenOrbCost)
        {
            StartCoroutine(SharpenRoutine());
        }
        else
        {
            Debug.Log("Memory Orb tidak cukup untuk sharpening! Butuh: " + sharpenOrbCost + " Orb.");
        }
    }

    IEnumerator SharpenRoutine()
    {
        Debug.Log("Mulai sharpening... Karakter stuck selama " + sharpenDuration + " detik.");
        
        isSharpening = true;
        if (playerMovement != null) playerMovement.isSharpening = true;

        currentMemoryOrbs -= sharpenOrbCost;

        yield return new WaitForSeconds(sharpenDuration);

        currentSharpness = maxSharpness;
        isSharpening = false;
        if (playerMovement != null) playerMovement.isSharpening = false;

        Debug.Log("Sharpening Selesai! Karakter bisa jalan lagi. Ketajaman: 100%");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}