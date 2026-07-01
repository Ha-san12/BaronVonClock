using UnityEngine;

public class MountSystem : MonoBehaviour
{
    [Header("Mount Settings")]
    [SerializeField] private Animator playerAnim; // Tarik komponen Animator Baron ke sini
    [SerializeField] private float mountSpeed = 8f; // Kecepatan ngacir pas naik Son
    private float kecepatanAsli;

    private bool bisaNaik = false;
    private bool lagiNaikSon = false;
    private GameObject objekSonBebas; // Nyimpen data ayam yang ada di dekat player

    // Pastikan lu punya referensi ke script movement player lu
    // private PlayerMovement playerMove; 

    void Start()
    {
        // Simpan kecepatan awal Baron biar bisa dibalikin pas turun
        // kecepatanAsli = playerMove.speed; 
    }

        void Update()
        {
            // Kalau lu di dekat ayam, pencet E, dan belum naik
            if (bisaNaik && Input.GetKeyDown(KeyCode.E) && !lagiNaikSon)
            {
                ProsesNaikSon();
            }
        }

    void ProsesNaikSon()
    {
        lagiNaikSon = true;

        // 1. Hilangkan ayam yang di map
        if (objekSonBebas != null)
        {
            objekSonBebas.SetActive(false); 
        }

        // 2. Mainkan SFX naik
        // AudioManager.instance.PlaySFX("naik son");

        // 3. Ubah animasi Baron ke wujud gabungan
        playerAnim.SetBool("isRiding", true);

        // 4. Ganti kecepatan lari (Sesuaikan dengan script movement lu)
        // playerMove.speed = mountSpeed;
    }

    // Mendeteksi pas Baron masuk area badan Son
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Son"))
        {
            bisaNaik = true;
            objekSonBebas = collision.gameObject;
        }
    }

    // Mendeteksi pas Baron menjauh dari Son
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Son"))
        {
            bisaNaik = false;
            objekSonBebas = null;
        }
    }
}