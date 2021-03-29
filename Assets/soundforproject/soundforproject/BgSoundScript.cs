using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgSoundScript : MonoBehaviour
{
    //void this for intialisation
    private void Start()
    {

    }

    //play Global

    private static BgSoundScript instance = null;
    private static BgSoundScript Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    //play Global end
    //update is called once per frame
    void Update()
    {

    }
}