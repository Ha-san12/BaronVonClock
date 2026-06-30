using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Volume State")]
    public int currentVolume = 3; // Asumsi maksimal 3 balok
    public int maxVolume = 3;

    [Header("UI Elements")]
    public Image volumeIndicatorImage; 
    public Sprite[] volumeSprites; // Masukkan gambar balok dari 0 (kosong) sampai penuh

    void Start()
    {
        ApplyVolume();
    }

    // Fungsi untuk tombol Plus (+)
    public void IncreaseVolume()
    {
        if (currentVolume < maxVolume)
        {
            currentVolume++;
            ApplyVolume();
        }
    }

    // Fungsi untuk tombol Minus (-)
    public void DecreaseVolume()
    {
        if (currentVolume > 0)
        {
            currentVolume--;
            ApplyVolume();
        }
    }

    void ApplyVolume()
    {
        // 1. Update gambar balok di UI
        if (volumeSprites.Length > 0 && currentVolume < volumeSprites.Length)
        {
            volumeIndicatorImage.sprite = volumeSprites[currentVolume];
        }

        // 2. Mengubah volume utama seluruh game (skala 0.0 sampai 1.0)
        AudioListener.volume = (float)currentVolume / maxVolume;
    }
}