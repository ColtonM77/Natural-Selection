using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.UI;

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

    // ammo display variables
    public bool isFiring;
    public Text ammoDisplay;

    private Animator anim;

    //Weapon variables
    private int selectedWeaponLocal = 1;
    public GameObject[] weaponArray;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;

    private Weapon activeWeapon;
    private float weaponCooldownTime;

    public Transform canvasBoard;

    //endgame
    public bool isDead = false;
    public bool hasWon = false;
    public bool isPlaying = true;

    //end game stuff
    [SerializeField]
    private GameObject lostGame;

    [SerializeField]
    private GameObject wonGame;

    //turn based
    public bool IsTurn { get { return PlayerManager.singleton.IsMyTurn(playerId); } }

    public int playerId;

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

        if (!IsTurn) return;

        if (isDead) return;

        if (hasWon) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        anim.SetBool("isGrounded", isGrounded);

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
            //activeWeapon.GetComponentInChildren<SpriteRenderer>().flipY = false;
            //Method 1
            FlipWeapon();
            //Method 2
            // GameObject temp = GameObject.Find("GunHolder");
            //Transform GunHolderTransform = temp.GetComponent<Transform>();
            //GunHolderTransform.localScale = new Vector3(GunHolderTransform.localScale.x *, GunHolderTransform.localScale.y * -1, GunHolderTransform.localScale.z);
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
            //activeWeapon.GetComponentInChildren<SpriteRenderer>().flipY = true;
            //Method 1
            FlipWeapon();
            //Method 2
            //GameObject temp3 = GameObject.Find("GunHolder");
            //Transform GunHolderTransform = temp3.GetComponent<Transform>();
            //GunHolderTransform.localScale = new Vector3(GunHolderTransform.localScale.x * -1, GunHolderTransform.localScale.y, GunHolderTransform.localScale.z);
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

        if (!IsTurn) return;

        if (isDead)
        {
            lostGame.SetActive(true);
            return;
        }

        if (hasWon)
        {
            wonGame.SetActive(true);
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (facingRight == false)
            {
                FlipWeapon();
            }

            selectedWeaponLocal += 1;

            if (selectedWeaponLocal > weaponArray.Length - 1)
            {
                selectedWeaponLocal = 1;
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);

            if (facingRight == false)
            {
                FlipWeapon();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (facingRight == false)
            {
                FlipWeapon();
            }

            selectedWeaponLocal -= 1;

            if (selectedWeaponLocal < 1)
            {
                selectedWeaponLocal = weaponArray.Length - 1;
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);

            if (facingRight == false)
            {
                FlipWeapon();
            }
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        activeWeapon.direction = mousePos - (Vector2)activeWeapon.Gun.position;
        FaceMouse();

        ammoDisplay.text = activeWeapon.weaponAmmo.ToString();

        if (Input.GetMouseButtonDown(0) && !isFiring) //Fire1 is mouse 1st click
        {
            if (activeWeapon && Time.time > weaponCooldownTime && activeWeapon.weaponAmmo > 0)
            {
                weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                //sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                CmdShootRay();
                if (IsTurn)
                    PlayerManager.singleton.NextPlayer();
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
        isFiring = true;
        activeWeapon.weaponAmmo--;
        isFiring = false;

        GameObject BulletIns = Instantiate(activeWeapon.Bullet, activeWeapon.ShootPoint.position, activeWeapon.ShootPoint.rotation);
        BulletIns.GetComponent<Rigidbody2D>().AddForce(BulletIns.transform.right * activeWeapon.BulletSpeed);
        if (BulletIns) { Destroy(BulletIns, activeWeapon.weaponLife); }
        activeWeapon.gunAnimator.SetTrigger("Shoot");
        //CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
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

    void FlipWeapon()
    {
        Transform activeWeaponTransform = activeWeapon.GetComponentInChildren<Transform>();
        activeWeaponTransform.localScale = new Vector3(activeWeaponTransform.localScale.x, activeWeaponTransform.localScale.y * -1, activeWeaponTransform.localScale.z);
    }
}
