using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;

    //[SyncVar(hook = nameof(HandleHealthUpdated))]
    public int currentHealth;

    public HealthBar healthBar;

    private PlayerController player;
    //public static event EventHandler<DeathEventArgs> OnDeath;
    //public event EventHandler<HealthChangedEventArgs> OnHealthChanged;

    //public bool IsDead => currentHealth == 0;

    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        player = GetComponent<PlayerController>();
    }

    /*
    [ServerCallback]
    private void OnDestroy()
    {
        OnDeath?.Invoke(this, new DeathEventArgs { ConnectionToClient = connectionToClient });
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            //OnDeath?.Invoke(this, new DeathEventArgs { ConnectionToClient = connectionToClient }); ;
            //RpcHandleDeath();
            //gameObject.SetActive(false);
            player.isDead = true;
        }
    }



    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.GetComponent<Bullet>())
        {
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    /*
    private void HandleHealthUpdated(int oldValue, int newValue)
    {
        OnHealthChanged?.Invoke(this, new HealthChangedEventArgs
        {
            Health = currentHealth,
            MaxHealth = maxHealth
        }); 
    }
    */

    /*
    [ClientRpc]
    void RpcHandleDeath()
    {
        gameObject.SetActive(false);
    }
    */
}

    