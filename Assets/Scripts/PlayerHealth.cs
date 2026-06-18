using UnityEngine;
using System.Collections.Generic; // Dibutuhkan jika ada error koleksi data
using UnityEngine.SceneManagement; // Penting untuk me-restart level otomatis

public class PlayerHealth : MonoBehaviour
{
    [Header("Son Cooldown Settings")]
    public float sonCooldownDuration = 5f; // Durasi ayam kabur (detik)
    private float cooldownTimer = 0f;
    private bool isCooldownActive = false;

    private PlayerMovement playerMovement;

    void Start()
    {
        // Mengambil referensi dari script PlayerMovement yang satu objek dengan script ini
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Jalankan timer jika Son sedang dalam masa cooldown
        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                EndSonCooldown();
            }
        }
    }

    // Fungsi utama yang bakal dipanggil kalau Baron terkena serangan musuh
    public void TakeDamage()
    {
        // KONDISI 1: Jika sedang naik ayam (Mounted)
        if (playerMovement.isMounted)
        {
            Debug.Log("EVENT: Lunch Break Over! Son terkena hit dan kabur!");
            
            // Turunkan Baron dari ayam (Ubah state pergerakan jadi lambat & kesat)
            playerMovement.SetMovementState(false);
            
            // Mulai masa cooldown ayam
            StartSonCooldown();
        }
        // KONDISI 2: Jika sedang jalan kaki (Dismounted) dan kena hit
        else
        {
            if (isCooldownActive)
            {
                // Jika Son masih cooldown, berarti Baron murni jalan kaki -> Mati!
                TriggerPlayerDeath();
            }
        }
    }

    void StartSonCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = sonCooldownDuration;
        Debug.Log("Son Cooldown Dimulai selama: " + sonCooldownDuration + " detik.");
        
        // TODO: Nanti di sini tempat lu mainin sound effect ayam panik / musik berubah pelan
    }

    void EndSonCooldown()
    {
        isCooldownActive = false;
        // Kembalikan otomatis ke mode naik ayam setelah cooldown selesai
        playerMovement.SetMovementState(true);
        Debug.Log("Son telah kembali! Baron otomatis menunggangi Son.");
    }

    void TriggerPlayerDeath()
    {
        Debug.Log("EVENT: FIRED! Baron mati terkena hit saat jalan kaki!");
        
        // TODO: Nanti pasang efek layar kaca pecah atau rontok jadi gir di sini

        // Mengulang lantai/level saat ini secara instan sesuai GDD agar fast-paced
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // CHEAT/TESTING: Tekan tombol 'K' di keyboard untuk pura-pura kena hit musuh
    // void OnGUI()
    // {
    //     GUI.Box(new Rect(10, 10, 250, 40), "Tekan 'K' untuk Simulasi Kena Hit");
    //     if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.K)
    //     {
    //         TakeDamage();
    //     }
    // }
}