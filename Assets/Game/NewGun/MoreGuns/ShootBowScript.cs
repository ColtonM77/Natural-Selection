using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
//using CodeMonkey.Utils;


public class ShootBowScript : MonoBehaviour
{
    public Transform Bow;
    public Animator BowAnimator;
    Vector2 direction;
    public GameObject Arrow;
    public float ArrowSpeed;
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

    

    //variables for arrow tracking 
    public GameObject PointPrefab;
    public GameObject[] Points;
    public int numberOfPoints;
    public float spaceBetweenPointsPoints;

    //delete game points variable

    public bool DeleteAimPoints;
    public WeaponScript weaponScript;

    //private Testing testing;
    //variable projectile speed variables
    private float holdDownStartTime;
    public float MAX_FORCE = 200f;

    //power bar variables
    //private Animator powerBarAnimator;
    //public GameObject powerBarMask;




    void Start()
    {
        currentAmmo = maxAmmo;

        /*Points = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            Points[i] = Instantiate(PointPrefab, transform.position, Quaternion.identity);
        }*/
       // powerBarAnimator = powerBarMask.GetComponent<Animator>(); 

    }

    void Awake()
    {
        weaponScript = GameObject.FindObjectOfType<WeaponScript>();
        // testing = GameObject.FindObjectOfType<Testing>();

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
        direction = mousePos - (Vector2)Bow.position;
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

        //showPoints();



    }


    /*public void ShowForce(float force)
    {
        Force.SpriteMask.alphaCutoff = 1 - force / MAX_FORCE;
    }*/

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

    //Bow facese mouse
    void FaceMouse()
    {
        Bow.transform.right = direction;
    }
    //shoot Bow and Bowbehaviour
    public void shoot(float Speed)
    {
        //ammo
        currentAmmo--;

        GameObject ArrowIns = Instantiate(Arrow, ShootPoint.position, ShootPoint.rotation);
        ArrowIns.GetComponent<Rigidbody2D>().velocity = transform.right * Speed;
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        Destroy(ArrowIns, 25);
    }

    public float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 5f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * MAX_FORCE;
        return force;
    }

    /*  Vector2 PointPosition(float t)
      {
          Vector2 currentPointPos = (Vector2)transform.position + (direction.normalized * ArrowSpeed * t) + 0.5f * Physics2D.gravity * (t * t);
          return currentPointPos;
      }



    public void showPoints()
    {
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i].transform.position = PointPosition(i * spaceBetweenPointsPoints);
        }

    }*/


}
