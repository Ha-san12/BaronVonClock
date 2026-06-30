using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Element References")]
    // Ubah penamaan dari bar ke image karena kita akan mengganti sprite-nya
    public Image weaponSharpnessImage; 
    
    // Array untuk menampung 5 gambar pedang (100%, 75%, 50%, 25%, 0%)
    public Sprite[] swordSprites; 
    
    public Image sonMeterImage;       // Objek UI_SonMeter[cite: 7]
    public TextMeshProUGUI orbText;   //[cite: 7]

    [Header("Player Script References")]
    private PlayerAttack playerAttack; //[cite: 7]
    private PlayerHealth playerHealth; //[cite: 7]

    void Start()
    {
        GameObject player = GameObject.Find("Player_Baron"); //[cite: 7]
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>(); //[cite: 7]
            playerHealth = player.GetComponent<PlayerHealth>(); //[cite: 7]
        }
    }

    void Update()
    {
        if (playerAttack == null || playerHealth == null) return; //[cite: 7]

        // 1. UPDATE GAMBAR KETAJAMAN SENJATA (SPRITE SWAPPING)
        // Hitung persentase ketajaman[cite: 7]
        float sharpnessPercentage = playerAttack.currentSharpness / playerAttack.maxSharpness; 

        // Pastikan array gambar sudah diisi di Inspector agar tidak error
        if (swordSprites.Length == 5)
        {
            // Logika mengganti gambar sesuai persentase
            if (sharpnessPercentage >= 1f) {
                weaponSharpnessImage.sprite = swordSprites[0]; // 100%
            } 
            else if (sharpnessPercentage >= 0.75f) {
                weaponSharpnessImage.sprite = swordSprites[1]; // 75%
            } 
            else if (sharpnessPercentage >= 0.5f) {
                weaponSharpnessImage.sprite = swordSprites[2]; // 50%
            } 
            else if (sharpnessPercentage > 0f) {
                weaponSharpnessImage.sprite = swordSprites[3]; // 25% (Di atas 0%)
            } 
            else {
                weaponSharpnessImage.sprite = swordSprites[4]; // 0% (Hancur/Tumpul)
            }
        }

        // 2. Update Teks Jumlah Memory Orbs[cite: 7]
        orbText.text = playerAttack.currentMemoryOrbs.ToString() + "/7"; //[cite: 7]

        // 3. LOGIKA INDIKATOR SON METER (AYAM)[cite: 7]
        if (!playerHealth.isCooldownActive) //[cite: 7]
        {
            sonMeterImage.fillAmount = 1f; //[cite: 7]
            sonMeterImage.color = Color.white; //[cite: 7]
        }
        else //[cite: 7]
        {
            sonMeterImage.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);  //[cite: 7]
            float cooldownPercentage = (playerHealth.sonCooldownDuration - playerHealth.cooldownTimer) / playerHealth.sonCooldownDuration; //[cite: 7]
            sonMeterImage.fillAmount = cooldownPercentage; //[cite: 7]
        }
    }
}