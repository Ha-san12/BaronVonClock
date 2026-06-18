using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Son Cooldown Settings")]
    public float sonCooldownDuration = 5f; // Durasi ayam kabur (detik)
    public float cooldownTimer = 0f;
    public bool isCooldownActive = false;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        
        // PASTIKAN pas game mulai, Baron otomatis naik ayam sesuai GDD
        if (playerMovement != null)
        {
            playerMovement.SetMovementState(true);
        }
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

    // Fungsi utama saat Baron ditabrak musuh
    public void TakeDamage()
    {
        // KONDISI 1: Jika sedang naik ayam (Mounted)
        if (playerMovement.isMounted && !isCooldownActive)
        {
            Debug.Log("EVENT: Lunch Break Over! Son terkena hit dan kabur!");
            
            // Turunkan Baron dari ayam (Ubah pergerakan jadi lambat & kesat)
            playerMovement.SetMovementState(false);
            
            // Mulai masa cooldown ayam
            StartSonCooldown();
        }
        // KONDISI 2: Jika sedang jalan kaki (Dismounted) dan cooldown masih berjalan
        else if (!playerMovement.isMounted && isCooldownActive)
        {
            // Sumpah ini pelindung biar ga double-hit instan dalam 1 frame semenjak ayam kabur
            if (cooldownTimer < (sonCooldownDuration - 0.5f)) 
            {
                TriggerPlayerDeath();
            }
        }
    }

    void StartSonCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = sonCooldownDuration;
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
        
        // Mengulang level secara instan
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}