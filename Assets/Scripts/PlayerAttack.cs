using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public enum WeaponType
    {
        ToySword,
        BalloonSword
    }

    [Header("Weapon Status")]
    public WeaponType currentWeapon = WeaponType.ToySword;

    [Header("Toy Sword Settings")]
    public float toyAttackRange = 1.2f;
    public float toyMaxSharpness = 100f;
    public float toyLossPerHit = 6.6f;
    public float toyCurrentSharpness = 100f;

    [Header("Balloon Sword Settings")]
    public float balloonAttackRange = 2.5f;
    public float balloonMaxSharpness = 100f;
    public float balloonLossPerHit = 34f;
    public float balloonCurrentSharpness = 100f;

    [Header("General Attack Settings")]
    public Transform attackPoint;           
    public LayerMask enemyLayer;            
    public int currentMemoryOrbs = 5;       
    public int sharpenOrbCost = 2;          
    public float sharpenDuration = 4f;      

    // =========================================================
    // FITUR BUNGLON: Agar UIManager tetap berfungsi tanpa error
    // =========================================================
    public float currentSharpness 
    {
        get { return currentWeapon == WeaponType.ToySword ? toyCurrentSharpness : balloonCurrentSharpness; }
        set { 
            if (currentWeapon == WeaponType.ToySword) toyCurrentSharpness = value; 
            else balloonCurrentSharpness = value; 
        }
    }

    public float maxSharpness 
    {
        get { return currentWeapon == WeaponType.ToySword ? toyMaxSharpness : balloonMaxSharpness; }
    }

    private float CurrentAttackRange 
    {
        get { return currentWeapon == WeaponType.ToySword ? toyAttackRange : balloonAttackRange; }
    }

    private float CurrentLossPerHit
    {
        get { return currentWeapon == WeaponType.ToySword ? toyLossPerHit : balloonLossPerHit; }
    }
    // =========================================================

    private bool isSharpening = false;      
    private PlayerMovement playerMovement;  
    private Animator anim;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        if (isSharpening) return;

        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetKeyDown(KeyCode.R)) TrySharpenWeapon();

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(WeaponType.ToySword);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(WeaponType.BalloonSword);
    }

    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
        
        if (currentWeapon == WeaponType.ToySword)
        {
            if (anim != null) anim.SetFloat("WeaponID", 0f);
            Debug.Log("Equipped: Toy Sword | Ketajaman Tersisa: " + toyCurrentSharpness);
        }
        else if (currentWeapon == WeaponType.BalloonSword)
        {
            if (anim != null) anim.SetFloat("WeaponID", 1f);
            Debug.Log("Equipped: Balloon Sword | Ketajaman Tersisa: " + balloonCurrentSharpness);
        }
    }

    void Attack()
    {
        if (currentSharpness <= 0f)
        {
            Debug.Log("Senjata Rusak! Tekan 'R'!");
            return;
        }

        if (anim != null) anim.SetTrigger("Attack");

        // Jika attackPoint tidak sengaja kosong, gunakan posisi badan karakter sebagai pusat serangan
        Vector2 attackPos = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, CurrentAttackRange, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            currentSharpness -= CurrentLossPerHit;
            currentSharpness = Mathf.Clamp(currentSharpness, 0f, maxSharpness);
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            Destroy(enemy.gameObject); 
        }
    }

    void TrySharpenWeapon()
    {
        if (currentSharpness >= maxSharpness) return;
        if (currentMemoryOrbs >= sharpenOrbCost) StartCoroutine(SharpenRoutine());
    }

    IEnumerator SharpenRoutine()
    {
        isSharpening = true;
        if (playerMovement != null) playerMovement.isSharpening = true;
        currentMemoryOrbs -= sharpenOrbCost;
        
        yield return new WaitForSeconds(sharpenDuration);
        
        currentSharpness = maxSharpness; 
        
        isSharpening = false;
        if (playerMovement != null) playerMovement.isSharpening = false;
    }

    // MENGGUNAKAN ONDRAWGIZMOS: Akan SELALU digambar meskipun objek tidak diklik
    void OnDrawGizmos()
    {
        // Fitur aman: Jika attack point kosong, gambar di tengah karakter
        Vector2 drawPos = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;

        // 1. Gambar range Toy Sword (Warna Kuning)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(drawPos, toyAttackRange);

        // 2. Gambar range Balloon Sword (Warna Cyan/Biru Muda)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(drawPos, balloonAttackRange);

        // 3. Gambar range senjata yang SEDANG DIPAKAI (Merah Transparan, hanya saat Play)
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(drawPos, CurrentAttackRange);
        }
    }
}