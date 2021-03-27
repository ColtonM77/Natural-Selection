using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CodeMonkey.Utils;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    bool hasHit;

    //variable speed
    public const float MAX_Force = 500f;

    public void Launch(float force)
    { }

    public void ShowForce(float force)
    { }





    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        //prints to console the name of the game object that was hit
        Debug.Log(hitInfo.name);
        Destroy(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

        // Update is called once per frame
    void Update()
    {
        if (hasHit == false)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }
}