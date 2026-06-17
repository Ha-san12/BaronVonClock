using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Mengambil referensi kamera utama di scene
        mainCamera = Camera.main;
    }

    void Update()
    {
        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        // 1. Dapatkan posisi kursor mouse di dalam dunia game (World Position)
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. Hitung arah dari posisi karakter ke posisi mouse
        Vector2 lookDirection = (Vector2)mousePosition - (Vector2)transform.position;

        // 3. Hitung sudut rotasi dalam derajat menggunakan Matematika Trigonometri (Atan2)
        // Di 2D, arah default sprite biasanya menghadap ke KANAN (0 derajat)
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // 4. Terapkan rotasi tersebut ke sumbu Z karakter
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}