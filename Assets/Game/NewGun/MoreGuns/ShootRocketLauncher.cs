using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.UI;


public class ShootRocketLauncher : MonoBehaviour
{

    public Transform Gun;
    public Animator gunAnimator;
    Vector2 direction;
    public GameObject Bullet;
    public float BulletSpeed;
    public Transform ShootPoint;
    public float fireRate;
    private float ReadyForNextShot;
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    //amo display variables
    public int ammo;
    public bool isFiring;
    public Text ammoDisplay;

    //ammo and reloading variables
    private int currentAmmo;
    public float reloadTime = 1f;
    public int maxAmmo;
    private bool isReloading = false;
    public Animator animator;

    //variable projectile speed variables
    private float holdDownStartTime;
    public float MAX_FORCE = 200f;



    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    //stops weapons not working when weapons are switched during a reload
    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;

        //ammo and reloading
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());

            return;
        }

        //ammoDisplay.text = ammo.ToString();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePos - (Vector2)Gun.position;
        FaceMouse();

        if (Input.GetMouseButtonDown(0))
        {
            //mouse Down, start holding 
            holdDownStartTime = Time.time;
            // powerBarAnimator.SetBool("active", true);


        }

        if (Input.GetMouseButton(0))
        {

            //mouse still down show force 
            //showPoints();
            float holdDownTime = Time.time - holdDownStartTime;
            CalculateHoldDownForce(holdDownTime);


        }

        if (Input.GetMouseButtonUp(0) && !isFiring && ammo > 0)
        {
            //mouse up launch
            //delay time till next shot
            if (Time.time > ReadyForNextShot)
            {
                ReadyForNextShot = Time.time + 1 / fireRate;
                float holdDownTime = Time.time - holdDownStartTime;
                shoot(CalculateHoldDownForce(holdDownTime));
                // powerBarAnimator.SetBool("active", false);
                isFiring = true;
                ammo--;
                isFiring = false;
            }
        }
        ammoDisplay.text = ammo.ToString();

    }

    //handles reloading 
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");

        animator.SetBool("Reloading", true);

        //waiting for the reload time
        yield return new WaitForSeconds(reloadTime - 0.25f);

        animator.SetBool("Reloading", false);

        yield return new WaitForSeconds(0.25f);


        currentAmmo = maxAmmo;
        isReloading = false;
    }

    //gun facese mouse
    void FaceMouse()
    {
        Gun.transform.right = direction;
    }
    //shoot gun and gunbehaviour
    public void shoot(float Speed)
    {
        //ammo
        currentAmmo--;

        GameObject BulletIns = Instantiate(Bullet, ShootPoint.position, ShootPoint.rotation);
        BulletIns.GetComponent<Rigidbody2D>().velocity = transform.right * Speed;
        gunAnimator.SetTrigger("Shoot");
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        Destroy(BulletIns, 25);
    }

    public float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 3f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * MAX_FORCE;
        return force;
    }
}
