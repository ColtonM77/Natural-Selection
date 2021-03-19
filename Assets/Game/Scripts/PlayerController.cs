using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class PlayerController : NetworkBehaviour
{

    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJumps;
    public int extraJumpsValue;


    private Animator anim;

    //Weapon variables
    private int selectedWeaponLocal = 1;
    public GameObject[] weaponArray;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;

    private Weapon activeWeapon;
    private float weaponCooldownTime;

    public Transform canvasBoard;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            extraJumps = extraJumpsValue;
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);


        moveInput = Input.GetAxisRaw("Horizontal");
        //Debug.Log(moveInput);
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (moveInput == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }

        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }


    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            // make non-local players run this
            //floatingInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown(KeyCode.W))) && extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKeyDown(KeyCode.E)) //Fire2 is mouse 2nd click and left alt
        {
            selectedWeaponLocal += 1;

            if (selectedWeaponLocal > weaponArray.Length-1)
            {
                selectedWeaponLocal = 1;
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);
        }

        if (Input.GetKeyDown(KeyCode.Q)) //Fire2 is mouse 2nd click and left alt
        {
            selectedWeaponLocal -= 1;

            if (selectedWeaponLocal < 1)
            {
                selectedWeaponLocal = weaponArray.Length-1;
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        activeWeapon.direction = mousePos - (Vector2)activeWeapon.Gun.position;
        FaceMouse();

        if (Input.GetButtonDown("Fire1")) //Fire1 is mouse 1st click
        {
            if (activeWeapon && Time.time > weaponCooldownTime && activeWeapon.weaponAmmo > 0)
            {
                weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                activeWeapon.weaponAmmo -= 1;
                //sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                CmdShootRay();
            }
        }
    }

    void OnWeaponChanged(int _Old, int _New)
    {
        // disable old weapon
        // in range and not null
        if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
        {
            weaponArray[_Old].SetActive(false);
        }

        // enable new weapon
        // in range and not null
        if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
        {
            weaponArray[_New].SetActive(true);
            activeWeapon = weaponArray[activeWeaponSynced].GetComponent<Weapon>();
            //if (isLocalPlayer) { sceneScript.UIAmmo(activeWeapon.weaponAmmo); }
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex;
    }

    void Awake()
    {
        // disable all weapons
        foreach (var item in weaponArray)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }

        if (selectedWeaponLocal < weaponArray.Length && weaponArray[selectedWeaponLocal] != null)
        { activeWeapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>(); /*sceneScript.UIAmmo(activeWeapon.weaponAmmo);*/ }
    }

    [Command]
    void CmdShootRay()
    {
        RpcFireWeapon();
    }

    [ClientRpc]
    void RpcFireWeapon()
    {
        /*
        //bulletAudio.Play(); muzzleflash  etc
        var bullet = (GameObject)Instantiate(activeWeapon.weaponBullet, activeWeapon.weaponFirePosition.position, activeWeapon.weaponFirePosition.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.forward * activeWeapon.weaponSpeed;
        if (bullet) { Destroy(bullet, activeWeapon.weaponLife); }

        */
        //ammo
        activeWeapon.weaponAmmo--;

        GameObject BulletIns = Instantiate(activeWeapon.Bullet, activeWeapon.ShootPoint.position, activeWeapon.ShootPoint.rotation);
        BulletIns.GetComponent<Rigidbody2D>().AddForce(BulletIns.transform.right * activeWeapon.BulletSpeed);
        activeWeapon.gunAnimator.SetTrigger("Shoot");
        //CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        Destroy(BulletIns, 2);

    }

    //gun faces mouse
    void FaceMouse()
    {
        activeWeapon.Gun.transform.right = activeWeapon.direction;
    }

    void Flip()
    {
        if (!isLocalPlayer) return;
        //Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        /*
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        */
        transform.Rotate(0f, 180f, 0f);
        canvasBoard.transform.Rotate(0f, 180f, 0f);
    }
}
