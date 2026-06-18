using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float maxSharpness = 100f;
    public float currentSharpness = 100f;
    public float sharpnessLossPerHit = 20f; // 5 kali pukul langsung tumpul
    public int sharpenOrbCost = 2;          // Biaya Memory Orb untuk menajamkan pedang

    [Header("Attack Settings")]
    public float attackRange = 1.2f;        // Jarak jangkauan pukulan pedang
    public Transform attackPoint;           // Titik pusat serangan (di depan karakter)
    public LayerMask enemyLayer;            // Layer khusus untuk musuh

    [Header("Inventory Data")]
    public int currentMemoryOrbs = 5;       // Data sementara jumlah Orb player (buat tes)

    void Update()
    {
        // 1. Klik Kiri Mouse untuk Menyerang
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // 2. Tekan Tombol R untuk Menajamkan Senjata (Sharpening)
        if (Input.GetKeyDown(KeyCode.R))
        {
            TrySharpenWeapon();
        }
    }

    void Attack()
    {
        // Jika ketajaman habis (0), damage jadi 0 alias ga bisa bunuh musuh
        if (currentSharpness <= 0f)
        {
            Debug.Log("Senjata Tumpul! Damage 0. Tekan 'R' untuk menajamkan kembali!");
            // TODO: Mainkan sound effect sedih / sad trombone di sini
            return;
        }

        Debug.Log("Baron mengayunkan pedang!");

        // Kurangi ketajaman senjata setiap kali mengayun/memukul
        currentSharpness -= sharpnessLossPerHit;
        currentSharpness = Mathf.Clamp(currentSharpness, 0f, maxSharpness);
        Debug.Log("Ketajaman Senjata Sisa: " + currentSharpness + "%");

        // Deteksi musuh yang berada di dalam area lingkaran serangan
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Musuh terkena hit pedang plastik Baron!");
            // Hancurkan musuh secara instan sesuai sistem One-Hit Kill di GDD
            Destroy(enemy.gameObject); 
        }
    }

    void TrySharpenWeapon()
    {
        // Cek apakah ketajaman sudah penuh atau belum
        if (currentSharpness >= maxSharpness)
        {
            Debug.Log("Senjata masih tajam, tidak perlu di-sharpen!");
            return;
        }

        // Cek kecukupan Memory Orbs player
        if (currentMemoryOrbs >= sharpenOrbCost)
        {
            currentMemoryOrbs -= sharpenOrbCost;
            currentSharpness = maxSharpness;
            Debug.Log("Sharpening Berhasil! Sisa Orb: " + currentMemoryOrbs + " | Ketajaman: 100%");
        }
        else
        {
            Debug.Log("Memory Orb tidak cukup untuk sharpening! Butuh: " + sharpenOrbCost + " Orb.");
        }
    }

    // Gambar visual jangkauan attack di Unity Editor agar gampang diatur sudutnya
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}