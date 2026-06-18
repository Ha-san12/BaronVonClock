using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Element References")]
    public Image weaponSharpnessBar;
    public Image sonMeterImage;       // Objek UI_SonMeter
    public TextMeshProUGUI orbText;

    [Header("Player Script References")]
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth; // Ganti dari PlayerMovement ke PlayerHealth

    void Start()
    {
        GameObject player = GameObject.Find("Player_Baron");
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
            playerHealth = player.GetComponent<PlayerHealth>(); // Ambil script health
        }
    }

    void Update()
    {
        if (playerAttack == null || playerHealth == null) return;

        // 1. Update Bar Ketajaman Senjata
        float sharpnessPercentage = playerAttack.currentSharpness / playerAttack.maxSharpness;
        weaponSharpnessBar.fillAmount = sharpnessPercentage;

        // 2. Update Teks Jumlah Memory Orbs
        orbText.text = playerAttack.currentMemoryOrbs.ToString() + "/7";

        // 3. LOGIKA INDIKATOR SON METER (AYAM)
        // Jika ayam SIAP / AKTIF (tidak sedang dalam masa cooldown)
        if (!playerHealth.isCooldownActive)
        {
            sonMeterImage.fillAmount = 1f; // Lingkaran penuh
            sonMeterImage.color = Color.white; // Warna menyala/terang (Ayam aktif)
        }
        // Jika ayam KABUR (sedang dalam masa cooldown)
        else
        {
            // Ubah warna ikon jadi abu-abu meredup tanda ayam lagi kabur
            sonMeterImage.color = new Color(0.3f, 0.3f, 0.3f, 0.6f); 

            // Hitung persentase durasi cooldown yang sudah berjalan (0.0 sampai 1.0)
            // Mengisi lingkaran memutar seiring timer cooldown mendekati selesai
            float cooldownPercentage = (playerHealth.sonCooldownDuration - playerHealth.cooldownTimer) / playerHealth.sonCooldownDuration;
            sonMeterImage.fillAmount = cooldownPercentage;
        }
    }
}