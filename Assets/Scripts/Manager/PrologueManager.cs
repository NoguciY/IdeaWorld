using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : MonoBehaviour
{

    [Header("�Q�[���N���A�̃A�C�R��"), SerializeField]
    GameObject clearIcon;
    void Start()
    {
        if(ClearTheGame.clearTheGame.GameClear)
        {
            clearIcon.SetActive(true);
        }
    }

}
