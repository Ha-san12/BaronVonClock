using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D customCursor; // Masukkan gambar ikon aim di sini
    public Vector2 hotSpot = Vector2.zero; // Titik tengah kursor

    void Start()
    {
        if (customCursor != null)
        {
            // Menghitung titik tengah gambar agar kekerannya pas di tengah ujung mouse
            hotSpot = new Vector2(customCursor.width / 2f, customCursor.height / 2f);
            
            // Mengubah kursor bawaan menjadi kursor custom
            Cursor.SetCursor(customCursor, hotSpot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Gambar kursor belum dimasukkan di Inspector!");
        }
    }
}