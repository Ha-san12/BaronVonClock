using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ditambahkan untuk memanipulasi Scene

public class MainMenuManager : MonoBehaviour
{
    // Fungsi ini harus 'public' agar bisa dideteksi oleh tombol UI
    public void PlayGame()
    {
        Debug.Log("Memuat Scene: Gameplay...");
        // Pastikan nama di dalam tanda kutip sama persis dengan nama file Scene-mu
        SceneManager.LoadScene("Gameplay"); 
    }

    // Bonus: Sekalian kita buatkan fungsi untuk tombol Quit
    public void QuitGame()
    {
        Debug.Log("Game ditutup!");
        Application.Quit(); // Catatan: Ini hanya bekerja saat game sudah di-build, tidak bekerja di Editor
    }
}