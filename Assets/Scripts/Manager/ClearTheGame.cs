using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTheGame : MonoBehaviour
{
    public bool GameClear=false;

    public static ClearTheGame clearTheGame;

    private void Awake()
    {
        if(clearTheGame==null)
        {
            clearTheGame = this;
            DontDestroyOnLoad(clearTheGame);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
