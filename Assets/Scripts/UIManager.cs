using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Element References")]
    public Image weaponSharpnessImage; 
    
    [Header("Weapon Sprites")]
    public Sprite[] toySwordSprites;       
    public Sprite[] balloonSwordSprites;
    public Sprite[] grenadeSprites; // Tambahan slot untuk sprite Grenade
    
    [Header("Other UI Elements")]
    public Image sonMeterImage;
    public TextMeshProUGUI orbText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;

    void Start()
    {
        GameObject player = GameObject.Find("Player_Baron");
        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (playerAttack == null || playerHealth == null) return;

        // Tentukan sprite mana yang digunakan berdasarkan senjata
        Sprite[] currentSpritesToUse = null;
        if (playerAttack.currentWeapon == PlayerAttack.WeaponType.ToySword) currentSpritesToUse = toySwordSprites;
        else if (playerAttack.currentWeapon == PlayerAttack.WeaponType.BalloonSword) currentSpritesToUse = balloonSwordSprites;
        else if (playerAttack.currentWeapon == PlayerAttack.WeaponType.Grenade) currentSpritesToUse = grenadeSprites;

        // Update gambar ketajaman
        if (currentSpritesToUse != null && currentSpritesToUse.Length > 0 && playerAttack.maxSharpness > 0)
        {
            float sharpnessPercentage = playerAttack.currentSharpness / playerAttack.maxSharpness;
            int spriteIndex = Mathf.Clamp(Mathf.RoundToInt((1f - sharpnessPercentage) * (currentSpritesToUse.Length - 1)), 0, currentSpritesToUse.Length - 1);
            weaponSharpnessImage.sprite = currentSpritesToUse[spriteIndex];
        }

        if (orbText != null) orbText.text = playerAttack.currentMemoryOrbs.ToString() + "/7";

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
                sonMeterImage.fillAmount = (playerHealth.sonCooldownDuration - playerHealth.cooldownTimer) / playerHealth.sonCooldownDuration;
            }
        }
    }

    public void MunculkanGameOver() { if (gameOverPanel != null) gameOverPanel.SetActive(true); }
    public void Tombol_Retry() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void Tombol_Lanjut() { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }
}