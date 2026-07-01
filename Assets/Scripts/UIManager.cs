using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Penting untuk fungsi pindah scene

public class UIManager : MonoBehaviour
{
    [Header("UI Element References")]
    public Image weaponSharpnessImage; 
    
    [Header("Weapon Sprites")]
    public Sprite[] toySwordSprites;       
    public Sprite[] balloonSwordSprites;   
    
    [Header("Other UI Elements")]
    public Image sonMeterImage;
    public TextMeshProUGUI orbText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // Pastikan tarik panel Game Over-mu ke sini di Inspector

    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;

    void Start()
    {
        // Mencari referensi ke Player secara otomatis
        GameObject player = GameObject.Find("Player_Baron");
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        
        // Pastikan panel tersembunyi saat game baru dimulai
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (playerAttack == null || playerHealth == null) return;

        // 1. UPDATE GAMBAR KETAJAMAN SENJATA (Dinamis)
        float sharpnessPercentage = playerAttack.currentSharpness / playerAttack.maxSharpness; 
        
        Sprite[] currentSpritesToUse = playerAttack.currentWeapon == PlayerAttack.WeaponType.ToySword ? toySwordSprites : balloonSwordSprites;

        if (currentSpritesToUse != null && currentSpritesToUse.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(Mathf.RoundToInt((1f - sharpnessPercentage) * (currentSpritesToUse.Length - 1)), 0, currentSpritesToUse.Length - 1);
            weaponSharpnessImage.sprite = currentSpritesToUse[spriteIndex];
        }

        // 2. Update Teks Memory Orbs
        if (orbText != null) orbText.text = playerAttack.currentMemoryOrbs.ToString() + "/7";

        // 3. Update Son Meter
        if (sonMeterImage != null)
        {
            if (!playerHealth.isCooldownActive)
            {
                sonMeterImage.fillAmount = 1f;
                sonMeterImage.color = Color.white;
            }
            else
            {
                sonMeterImage.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
                float cooldownPercentage = (playerHealth.sonCooldownDuration - playerHealth.cooldownTimer) / playerHealth.sonCooldownDuration;
                sonMeterImage.fillAmount = cooldownPercentage;
            }
        }
    }

    // ==========================================
    // FUNGSI UNTUK PANEL GAME OVER
    // ==========================================
    public void MunculkanGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // Fungsi ini dipanggil melalui tombol Retry di Inspector
    public void Tombol_Retry()
    {
        Time.timeScale = 1f; // Wajib balikin waktu ke normal sebelum load scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    // Fungsi ini dipanggil melalui tombol Lanjut/Menu di Inspector
    public void Tombol_Lanjut()
    {
        Time.timeScale = 1f; // Wajib balikin waktu ke normal
        // Ganti "MainMenu" dengan nama scene menu utamamu
        SceneManager.LoadScene("MainMenu"); 
    }
}