using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    public float damage = 10f;       // Jumlah damage yang diberikan[cite: 3]
    public float lifetime = 3f;      // Waktu peluru hancur otomatis agar tidak lag[cite: 3]

    [Header("Visual Effects (Opsional)")]
    public GameObject hitEffectPrefab; // Masukkan prefab partikel debu/ledakan di sini

    void Start()
    {
        // Menghancurkan peluru secara otomatis setelah beberapa detik[cite: 3]
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Cek apakah mengenai Player[cite: 3]
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // Panggil fungsi kurangi HP[cite: 3]
            }

            DestroyBullet();
        }
        // 2. Cek apakah mengenai Tembok / Obstacle[cite: 3]
        // Pastikan nama layer tembokmu benar-benar "Wall" (huruf besar W)
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyBullet();
        }
    }

    // Fungsi khusus untuk menghancurkan peluru + memunculkan efek
    void DestroyBullet()
    {
        // Memunculkan efek benturan jika prefab-nya sudah diisi di Inspector
        if (hitEffectPrefab != null)
        {
            // Munculkan efek tepat di posisi peluru hancur
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            
            // Hancurkan efek visual tersebut setelah 1 detik agar tidak menumpuk
            Destroy(effect, 1f); 
        }

        // Hancurkan objek peluru ini[cite: 3]
        Destroy(gameObject);
    }
}