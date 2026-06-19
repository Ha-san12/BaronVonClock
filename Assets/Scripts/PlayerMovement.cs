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

    [Header("State")]
    public bool isMounted = true; 
    public bool isSharpening = false;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        SetMovementState(isMounted);
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.magnitude > 1)
        {
            movementInput.Normalize();
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
            Debug.Log("Mode: Mounted (Naik Son) - Cepat & Licin!");
        }
        else
        {
            currentSpeed = dismountedSpeed;
            activeSlippiness = dismountedSlippiness; 
            Debug.Log("Mode: Dismounted (Jalan Kaki) - Lambat & Presisi!");
        }
    }
}