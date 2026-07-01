using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Son Cooldown Settings")]
    public float sonCooldownDuration = 5f;
    public float cooldownTimer = 0f;
    public bool isCooldownActive = false;

    private PlayerMovement playerMovement;
    private Animator anim;
    private PlayerAttack playerAttack;
    private bool isDead = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        
        if (playerMovement != null)
        {
            playerMovement.SetMovementState(true);
        }
    }

    void Update()
    {
        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                EndSonCooldown();
            }
        }
    }

    public void TakeDamage()
    {
        if (isDead) return; // Jangan damage kalau sudah mati

        // KONDISI 1: Jika sedang naik ayam (Mounted)
        if (playerMovement.isMounted && !isCooldownActive)
        {
            playerMovement.SetMovementState(false);
            StartSonCooldown();
        }
        // KONDISI 2: Jika sedang jalan kaki (Dismounted)[cite: 2]
        else if (!playerMovement.isMounted && isCooldownActive)
        {
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
        playerMovement.SetMovementState(true);
    }

    // --- LOGIKA KEMATIAN BARU ---
    void TriggerPlayerDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("EVENT: Baron mati!");
        
        // 1. Pause Waktu Game
        Time.timeScale = 0f;

        // 2. Mainkan animasi mati sesuai senjata yang dipegang
        if (anim != null && playerAttack != null)
        {
            if (playerAttack.currentWeapon == PlayerAttack.WeaponType.ToySword)
                anim.SetTrigger("DieToy");
            else
                anim.SetTrigger("DieBalloon");
        }

        // 3. Panggil penghitung mundur untuk memunculkan panel Game Over
        StartCoroutine(TungguAnimasiMatiSelesai());
    }

    IEnumerator TungguAnimasiMatiSelesai()
    {
        // Tunggu animasi selesai meski game di-pause (pake Realtime)
        yield return new WaitForSecondsRealtime(1.5f); // Sesuaikan dengan durasi animasi mati

        // Cari UIManager dan munculkan panel Game Over
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null) ui.MunculkanGameOver();
    }
}