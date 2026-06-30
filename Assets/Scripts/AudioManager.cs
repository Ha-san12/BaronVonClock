using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Instance tunggal biar bisa dipanggil dari script mana pun (Singleton)
    public static AudioManager instance;

    [System.Serializable]
    public class Sound
    {
        public string namaSFX;      // Nama buat dipanggil di kodingan (misal: "cv_man_attack")
        public AudioClip fileAudio; // Masukin file .mp3 / .wav lu di sini
    }

    public Sound[] daftarSFX;       // Tempat nampung semua list sfx lu di Inspector
    private AudioSource sfxSource;  // Speaker khusus buat muter SFX

    void Awake()
    {
        // Setup Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Biar audio gak mati pas pindah level
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Bikin komponen AudioSource secara otomatis lewat kodingan
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    // FUNGSI UTAMA UNTUK MANGGIL SUARA
    public void PlaySFX(string nama)
    {
        Sound s = Array.Find(daftarSFX, sound => sound.namaSFX == nama);
        
        if (s == null)
        {
            Debug.LogWarning("SFX dengan nama: " + nama + " gak ketemu, San!");
            return;
        }

        // Muter suara sekali jepret (bisa tumpang tindih dengan aman kalau sfx-nya barengan)
        sfxSource.PlayOneShot(s.fileAudio);
    }
}