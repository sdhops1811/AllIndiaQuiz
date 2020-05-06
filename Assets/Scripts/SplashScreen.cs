using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject homeScreen;

    void Start()
    {
        Invoke("DelayStart", 2);
    }

    void DelayStart()
    {
        homeScreen.SetActive(true);
    }
    
}
