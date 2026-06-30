using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;   // Masukkan objek UI_PauseMenu (Jam) ke sini
    public GameObject settingsPanel; // Masukkan objek UI_SettingsPanel ke sini

    private bool isPaused = false;

    void Start()
    {
        // Pastikan semua UI tersembunyi dan waktu berjalan normal saat game dimulai
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 1f; 
    }

    void Update()
    {
        // Mengecek jika pemain menekan tombol ESC di keyboard
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Jika game sedang dalam keadaan pause
            if (isPaused)
            {
                // Kalau menu setting lagi terbuka, tombol ESC akan menutup setting (kembali ke jam)
                if (settingsPanel.activeInHierarchy)
                {
                    CloseSettings();
                }
                // Kalau menu setting tidak terbuka, tombol ESC akan melanjutkan game (resume)
                else
                {
                    ResumeGame();
                }
            }
            // Jika game sedang berjalan normal, pause game-nya
            else
            {
                PauseGame();
            }
        }
    }

    // --- FUNGSI UNTUK MENU JAM PAUSE ---

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);  // Munculkan UI Jam Pause
        Time.timeScale = 0f;          // Hentikan semua gerakan, animasi, dan fisika
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);   // Sembunyikan jam
        settingsPanel.SetActive(false); // Pastikan settings juga ikut tertutup
        Time.timeScale = 1f;            // Jalankan waktu kembali
        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        // PENTING: Wajib kembalikan waktu ke normal sebelum pindah scene
        Time.timeScale = 1f; 
        
        Debug.Log("Kembali ke Main Menu...");
        SceneManager.LoadScene("MainMenu"); // Pastikan nama scene menu-mu benar
    }


    // --- FUNGSI UNTUK MENU SETTINGS ---

    public void OpenSettings()
    {
        settingsPanel.SetActive(true); // Munculkan papan settings
        pauseMenuUI.SetActive(false);  // Sembunyikan jam sementara biar nggak tumpang tindih
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false); // Sembunyikan papan settings
        pauseMenuUI.SetActive(true);    // Munculkan jamnya lagi
    }
}