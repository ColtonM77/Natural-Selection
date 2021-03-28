using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.UI;

namespace Game
{
    public class Weapon : MonoBehaviour
    {
        
        public Transform Gun;
        public Animator gunAnimator;
        public Vector2 direction;
        public GameObject Bullet;
        public float BulletSpeed;
        public Transform ShootPoint;
        public float fireRate;
        private float ReadyForNextShot;
        public float magnitude;
        public float roughness;
        public float fadeInTime;
        public float fadeOutTime;

        //ammo display variables
        public int ammo;
        public bool isFiring;
        public Text ammoDisplay;

        //ammo and reloading variables
        private int currentAmmo;
        public float reloadTime = 1f;
        public int maxAmmo;
        //private bool isReloading = false;
        public Animator animator;
        
        //new weapon
        public float weaponSpeed = 15.0f;
        public float weaponLife = 3.0f;
        public float weaponCooldown = 1.0f;
        public int weaponAmmo = 15;

        public GameObject weaponBullet;
        public Transform weaponFirePosition;

        /*
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

            ammoDisplay.text = ammo.ToString();
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePos - (Vector2)Gun.position;
            FaceMouse();

            if (Input.GetMouseButton(0) && !isFiring && ammo > 0)
            {

                //delay time till next shot
                if (Time.time > ReadyForNextShot)
                {
                    ReadyForNextShot = Time.time + 1 / fireRate;
                    shoot();
                    isFiring = true;
                    ammo--;
                    isFiring = false;
                }
            }

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
        void shoot()
        {


            //ammo
            currentAmmo--;

            GameObject BulletIns = Instantiate(Bullet, ShootPoint.position, ShootPoint.rotation);
            BulletIns.GetComponent<Rigidbody2D>().AddForce(BulletIns.transform.right * BulletSpeed);
            gunAnimator.SetTrigger("Shoot");
            //CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
            Destroy(BulletIns, 2);

        }*/
    }
        
}
