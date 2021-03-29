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

    public bool hasShot = false;

    //sfx
    public AudioSource jumping;
    public AudioSource weaponSwitch;

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

        if (isDead)
        {
            lostGame.SetActive(true);
            return;
        }

        if (!IsTurn) return;

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
            jumping.Play();
            extraJumps--;
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown(KeyCode.W))) && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
            jumping.Play();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedWeaponLocal += 1;

            if (selectedWeaponLocal > weaponArray.Length - 1)
            {
                selectedWeaponLocal = 1;
            }

            weaponSwitch.Play();
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedWeaponLocal -= 1;

            if (selectedWeaponLocal < 1)
            {
                selectedWeaponLocal = weaponArray.Length - 1;
            }

            weaponSwitch.Play();
            CmdChangeActiveWeapon(selectedWeaponLocal);
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
                //hasShot = true;
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

        //PlayerManager.singleton.NextPlayer();

        activeWeapon.PlayAudio();

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
        FlipWeapon();
    }

    void FlipWeapon()
    {
        Transform weaponOneTransform = weaponArray[1].GetComponentInChildren<Transform>();
        weaponOneTransform.localScale = new Vector3(weaponOneTransform.localScale.x, weaponOneTransform.localScale.y * -1, weaponOneTransform.localScale.z);

        Transform weaponTwoTransform = weaponArray[2].GetComponentInChildren<Transform>();
        weaponTwoTransform.localScale = new Vector3(weaponTwoTransform.localScale.x, weaponTwoTransform.localScale.y * -1, weaponTwoTransform.localScale.z);

        Transform weaponThreeTransform = weaponArray[3].GetComponentInChildren<Transform>();
        weaponThreeTransform.localScale = new Vector3(weaponThreeTransform.localScale.x, weaponThreeTransform.localScale.y * -1, weaponThreeTransform.localScale.z);
    }
}
