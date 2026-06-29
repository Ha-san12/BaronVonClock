using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float currentSpeed = 5f;
    public float mountedSpeed = 8f;      
    public float dismountedSpeed = 4f;   

    [Header("Inertia / Slippery Settings")]
    public float activeSlippiness = 0.1f; 
    public float mountedSlippiness = 0.05f; 
    public float dismountedSlippiness = 0.4f; 

    [Header("Aiming Settings")]
    public float rotationOffset = 0f; // <--- UBAH NILAI INI DI INSPECTOR JIKA KARAKTER MASIH MIRING

    [Header("State")]
    public bool isMounted = true; 
    public bool isSharpening = false;

    private Rigidbody2D rb;
    private Animator anim; 
    private Vector2 movementInput;
    private Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        
        rb.gravityScale = 0f;
        SetMovementState(isMounted);
    }

    void Update()
    {
        // 1. Input Gerak
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.magnitude > 1)
        {
            movementInput.Normalize();
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", movementInput.magnitude);
        }

        // 2. Logika Rotasi mengikuti Cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePosition - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        // Menerapkan rotasi dengan offset agar sprite tegak lurus
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);

        if (Input.GetMouseButtonDown(0)) // 0 = Klik Kiri Mouse
        {
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity;

        if (isSharpening)
        {
            targetVelocity = Vector2.zero;
        }
        else
        {
            targetVelocity = movementInput * currentSpeed;
        }

        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, activeSlippiness);
    }

    public void SetMovementState(bool mounted)
    {
        isMounted = mounted;
        if (isMounted)
        {
            currentSpeed = mountedSpeed;
            activeSlippiness = mountedSlippiness; 
        }
        else
        {
            currentSpeed = dismountedSpeed;
            activeSlippiness = dismountedSlippiness; 
        }
    }
}