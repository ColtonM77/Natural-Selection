using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    PlayerController[] Players;

    //public Transform playerCamera;
    public static PlayerManager singleton;

    //[SyncVar(hook = nameof(MoveCamera))]
    [SyncVar]
    public int currentPlayer = -1;

    [SyncVar]
    public float MaxTurnTime;
    [SyncVar(hook = nameof(onTurnTimeChanged))]
    public float currentTurnTime;

    public Text turnTimeText;

    public Text playerTurnText;



    void onTurnTimeChanged(float _Old, float _New) { turnTimeText.text = "Time: " + _New.ToString("N0"); }

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        
        singleton = this;
        //playerCamera = Camera.main.transform;
    }


    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        Players = GameObject.FindObjectsOfType<PlayerController>();

        //playerCamera.SetParent(Players[currentPlayer].transform);
        //playerCamera.localPosition = Vector3.zero + Vector3.back * 10;

        if (isServer)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].playerId = i;
            }
            
            NextPlayer();
            currentTurnTime = MaxTurnTime;
        }
       

    }

    public void Update()
    {
        if (!isServer)
            return;

        currentTurnTime -= Time.deltaTime;

            if (currentTurnTime < 0)
            {

                StartCoroutine(NextPlayerCoroutine());
            }
    }

    public void NextPlayer()
    {
        if (!isServer)
            return;

        StartCoroutine(NextPlayerCoroutine());
    }

    public IEnumerator NextPlayerCoroutine()
    {

        var nextPlayer = currentPlayer + 1;
        currentTurnTime = MaxTurnTime;
        //currentPlayer = -1;

        yield return new WaitForSeconds(2);

        currentPlayer = nextPlayer;
        if (currentPlayer >= Players.Length)
        {
            currentPlayer = 0;
        }
    }

    /*
    void MoveCamera(int _Old, int _New)
    {
        currentPlayer = _New;

        if (_New < 0 || _New >= Players.Length)
            return;

        playerCamera.SetParent(Players[currentPlayer].transform);
        playerCamera.localPosition = Vector3.zero + Vector3.back * 10;
    }*/

    public bool IsMyTurn(int i)
    {
        return i == currentPlayer;
    }

}
