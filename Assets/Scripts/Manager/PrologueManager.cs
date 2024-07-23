using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : MonoBehaviour
{

    [Header("ゲームクリアのアイコン"), SerializeField]
    GameObject clearIcon;
    void Start()
    {
        if(ClearTheGame.clearTheGame.GameClear)
        {
            clearIcon.SetActive(true);
        }
    }

}
