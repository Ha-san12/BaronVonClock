using UnityEngine;
using UnityEngine.SceneManagement; // Wajib buat pindah scene

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // Biar gampang dipanggil dari script musuh

    private void Awake()
    {
        instance = this;
    }

    // Fungsi ini bakal dipanggil tiap kali ada musuh yang mati
    public void CekSisaMusuh()
    {
        // Ngabsen ada berapa objek di scene yang masih punya tag "Enemy"
        GameObject[] musuh = GameObject.FindGameObjectsWithTag("Enemy");

        if (musuh.Length == 0)
        {
            Debug.Log("Semua musuh habis! Pindah ke lantai selanjutnya...");
            PindahLantai();
        }
    }

    void PindahLantai()
    {
        // Cek kita lagi di urutan scene ke berapa sekarang
        int urutanSceneSekarang = SceneManager.GetActiveScene().buildIndex;
        
        // Pindah ke scene urutan selanjutnya (+1)
        SceneManager.LoadScene(urutanSceneSekarang + 1);
    }
}