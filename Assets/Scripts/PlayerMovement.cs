using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float currentSpeed = 5f;
    public float mountedSpeed = 8f;      // Kecepatan pas naik ayam (Son)
    public float dismountedSpeed = 4f;   // Kecepatan pas jalan kaki

    [Header("Inertia / Slippery Settings")]
    public float activeSlippiness = 0.1f; // Semakin kecil, semakin licin
    public float mountedSlippiness = 0.05f; // Licin banget pas naik ayam
    public float dismountedSlippiness = 0.4f; // Kesat/langsung berhenti pas jalan kaki

    [Header("State")]
    public bool isMounted = true; // Status apakah lagi naik ayam atau nggak

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 currentVelocity;

    void Start()
    {
        // Mengambil komponen Rigidbody2D dari game object ini
        rb = GetComponent<Rigidbody2D>();
        
        // Pastikan gravitasi di-set ke 0 karena ini game top-down
        rb.gravityScale = 0f;

        // Set status awal sesuai GDD (Mulai dengan naik Son)
        SetMovementState(isMounted);
    }

    void Update()
    {
        // 1. Ambil Input dari Keyboard (Arrow keys atau WASD)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Normalisasi input agar pergerakan diagonal tidak lebih cepat
        if (movementInput.magnitude > 1)
        {
            movementInput.Normalize();
        }

        // 2. CHEAT / TESTING: Tekan tombol Space buat simulasi ganti mode (Mounted / Dismounted)
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     isMounted = !isMounted;
        //     SetMovementState(isMounted);
        // }
    }

    void FixedUpdate()
    {
        // 3. Hitung target kecepatan berdasarkan input dan speed saat ini
        Vector2 targetVelocity = movementInput * currentSpeed;

        // 4. Efek Slippery (Inertia) menggunakan Vector2.SmoothDamp
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, activeSlippiness);
    }

    // Fungsi untuk mengubah status pergerakan berdasarkan GDD
    public void SetMovementState(bool mounted)
    {
        isMounted = mounted;
        if (isMounted)
        {
            currentSpeed = mountedSpeed;
            activeSlippiness = mountedSlippiness; // Jadi licin ala Hotline Miami
            Debug.Log("Mode: Mounted (Naik Son) - Cepat & Licin!");
        }
        else
        {
            currentSpeed = dismountedSpeed;
            activeSlippiness = dismountedSlippiness; // Jadi kesat/snappy buat stealth
            Debug.Log("Mode: Dismounted (Jalan Kaki) - Lambat & Presisi!");
        }
    }
}