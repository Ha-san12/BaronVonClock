using UnityEngine;
using UnityEngine.UI; // Penting untuk mengontrol komponen Image UI
using TMPro;        // Penting untuk mengontrol TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("UI Element References")]
    public Image weaponSharpnessBar; // Tarik objek UI_WeaponSharpness ke sini
    public TextMeshProUGUI orbText;   // Tarik objek Text_Orbs ke sini

    [Header("Player Script References")]
    private PlayerAttack playerAttack;

    void Start()
    {
        // Mencari script PlayerAttack yang menempel di objek Player_Baron
        GameObject player = GameObject.Find("Player_Baron");
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }
    }

    void Update()
    {
        if (playerAttack == null) return;

        // 1. Update Bar Ketajaman Senjata (Mengubah nilai 0-100% menjadi nilai Slider 0.0 - 1.0)
        float sharpnessPercentage = playerAttack.currentSharpness / playerAttack.maxSharpness;
        weaponSharpnessBar.fillAmount = sharpnessPercentage;

        // 2. Update Teks Jumlah Memory Orbs (Format: "JumlahSekarang/7")
        // Angka 7 kita hardcode dulu sementara sesuai wireframe Hanif
        orbText.text = playerAttack.currentMemoryOrbs.ToString() + "/7";
    }
}