using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureChest : MonoBehaviour
{
    [SerializeField]GameManager gameManager;

    [Header("UI�̕󔠂̃A�j���[�V�����̃I�u�W�F�N�g"),SerializeField]GameObject Takaraanimator;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(gameManager.keysRemaining==0)
            {
                Debug.Log("�N���A");
                //�v���C���[�̓������~�߂�
                collision.GetComponent<PlayerController>().playerMove = false;

                //�A�N�e�B�u�ɂ����āA�A�j���[�V�������Đ�����
                Takaraanimator.gameObject.SetActive(true);

                //�N���A�̃t���O
                ClearTheGame.clearTheGame.GameClear = true;
            }
            else
            {
                Debug.Log("�����K�v");
            }
        }
    }

}
