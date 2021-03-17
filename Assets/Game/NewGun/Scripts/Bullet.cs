using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public Rigidbody2D rb;
            public float destroyAfter = 5;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

  
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        //prints to console the name of the game object that was hit
        Debug.Log(hitInfo.name);
        //Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}