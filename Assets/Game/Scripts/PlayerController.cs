using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Movement variable
    public float speed;
    public float jump;
    float moveVelocity;

    //Grounded Variables
    bool grounded = true;

    void Update()
    {
        //Jumping code
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))// allows the user to player with different key layouts
        {
            if (grounded)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jump);
            }
        }

        moveVelocity = 0;

        //code to Move horizontaly
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveVelocity = -speed;
            
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveVelocity = speed;
            
        }

        GetComponent<Rigidbody2D>().velocity = new Vector2(moveVelocity, GetComponent<Rigidbody2D>().velocity.y);

    }
    //this section of code checks to see if the player is grounded
    void OnTriggerEnter2D()//the OnTriggerEnter2D method is a unity method that connects to a box collider
    {
        grounded = true;
    }
    void OnTriggerExit2D()
    {
        grounded = false;
    }

}
