using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("�Q��"),SerializeField]
    GameManager gameManager;

    void Start()
    {
        //gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        //�L�[�̐������C���N�������g���āA���v�����߂�
        gameManager.keysRemaining++;
    }

    //�v���C���[�Ƃ̐ڐG���̏���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //�t�����[�L�[�Q�b�g����
            gameManager.GetFlowerKey();
           
        }
    }

}
