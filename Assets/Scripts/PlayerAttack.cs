using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public enum WeaponType { ToySword, BalloonSword, Grenade }

    [Header("Weapon Status")]
    public WeaponType currentWeapon = WeaponType.ToySword;
    public List<WeaponType> unlockedWeapons; 

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

    public float currentSharpness 
    {
        get { return currentWeapon == WeaponType.ToySword ? toyCurrentSharpness : (currentWeapon == WeaponType.BalloonSword ? balloonCurrentSharpness : 100f); }
        set { if (currentWeapon == WeaponType.ToySword) toyCurrentSharpness = value; else if (currentWeapon == WeaponType.BalloonSword) balloonCurrentSharpness = value; }
    }
    public float maxSharpness => currentWeapon == WeaponType.ToySword ? toyMaxSharpness : balloonMaxSharpness;
    private float CurrentAttackRange => currentWeapon == WeaponType.ToySword ? toyAttackRange : balloonAttackRange;

    private bool isSharpening = false;      
    private Animator anim;

    void Start() { anim = GetComponent<Animator>(); EquipWeapon(currentWeapon); }

    void Update()
    {
        if (isSharpening) return;
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetKeyDown(KeyCode.R)) TrySharpenWeapon();

        if (Input.GetKeyDown(KeyCode.Alpha1) && unlockedWeapons.Contains(WeaponType.ToySword)) EquipWeapon(WeaponType.ToySword);
        if (Input.GetKeyDown(KeyCode.Alpha2) && unlockedWeapons.Contains(WeaponType.BalloonSword)) EquipWeapon(WeaponType.BalloonSword);
        if (Input.GetKeyDown(KeyCode.Alpha3) && unlockedWeapons.Contains(WeaponType.Grenade)) EquipWeapon(WeaponType.Grenade);
    }

    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
        if (anim != null) anim.SetFloat("WeaponID", currentWeapon == WeaponType.ToySword ? 0f : (currentWeapon == WeaponType.BalloonSword ? 1f : 2f));
    }

    void Attack()
    {
        if (currentWeapon == WeaponType.Grenade) return; // Mekanik grenade menyusul
        if (currentSharpness <= 0f) return;
        if (anim != null) anim.SetTrigger("Attack");

        Vector2 pos = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, CurrentAttackRange, enemyLayer);
        if (hits.Length > 0) currentSharpness = Mathf.Clamp(currentSharpness - (currentWeapon == WeaponType.ToySword ? toyLossPerHit : balloonLossPerHit), 0f, maxSharpness);
        foreach (Collider2D hit in hits) Destroy(hit.gameObject); 
    }

    void TrySharpenWeapon() { if (currentSharpness < maxSharpness && currentMemoryOrbs >= sharpenOrbCost) StartCoroutine(SharpenRoutine()); }

    IEnumerator SharpenRoutine()
    {
        isSharpening = true;
        currentMemoryOrbs -= sharpenOrbCost;
        yield return new WaitForSeconds(sharpenDuration);
        currentSharpness = maxSharpness; 
        isSharpening = false;
    }

    // ==========================================
    // FUNGSI GIZMO DIKEMBALIKAN
    // ==========================================
    void OnDrawGizmos()
    {
        // Fitur aman: Jika attack point kosong, gambar di tengah badan karakter
        Vector2 drawPos = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;

        // 1. Gambar range Toy Sword (Warna Kuning)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(drawPos, toyAttackRange);

        // 2. Gambar range Balloon Sword (Warna Biru Muda)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(drawPos, balloonAttackRange);

        // 3. Indikator range yang SEDANG DIPAKAI (Merah Transparan, hanya muncul saat Play)
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(drawPos, CurrentAttackRange);
        }
    }
}