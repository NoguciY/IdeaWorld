using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField,Tooltip("���݂̗̑�")] int life;        //�̗�
    [SerializeField,Tooltip("�̗͂̐ݒ�")] int maxLife;     //�ő�̗�
    [HideInInspector]public Vector2 checkPoint;             //�`�F�b�N�|�C���g�̏ꏊ���L�^
    [HideInInspector]public bool invincible=false;

    [Header("�Q�ƌn")]
    [SerializeField]GameManager gameManager;
    [SerializeField]MarkerManager markerManager;
    [SerializeField]PlayerController playerController;

    public GameObject optionButton;

    private void Start()
    {
        //�̗͂�ݒ�
        life = maxLife;

        //�`�F�b�N�|�C���g��ݒ�
        checkPoint = transform.position;
    }

    public void PlayerDamage(int damage)
    {
        //�̗͂����炷
        life -= damage;

        //���񂾏ꍇ�A�̗͂��񕜂����čŌ�ɒʉ߂����`�F�b�N�|�C���g�ŕ���
        if (life <= 0)
        {
            //�`���Ă�r���̐���`���̂��I������
            markerManager.PaintFinish();

            //�X�e�[�W���̂��ׂĂ̐�������
            markerManager.RemoveAllLines();

            gameManager.SwitchingMarkersAndSketches(true);

            //�̗͉�
            life = maxLife;

            //�ЂƂO�̃`�F�b�N�|�C���g�ɖ߂�
            transform.position = checkPoint;

            //�I�v�V�����{�^�����\������(�X�P�b�`���[�h�����񂾂Ƃ��̂���)
            optionButton.SetActive(true);
        }
    }

    //���݂̃��C�t���擾����֐�
    public int GetLife()
    {
        return life;
    }

    //�Ƃ��ɓ�����Ƒ���
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //���ɓ��������ꍇ
        if (collision.gameObject.tag == "DeadObject")
        {
            PlayerDamage(3);
        }
        //��ɓ��������ꍇ
        else if (collision.gameObject.tag == "Rock")
        {
            PlayerDamage(1);
        }
    }

    //�`�F�b�N�|�C���g�ɖ߂�
    public void ReturnCheckpoint()
    {
        //�v���C���[���Ԃ牺�����Ԃ̏ꍇ
        if (playerController.IsHanging)
        {
            //���D��j�󂷂�
            playerController.OnDestroyBalloonFlag(checkPoint);
        }

        markerManager.PaintFinish();
        gameManager.SwitchingMarkersAndSketches(true);
        life = maxLife;
        transform.position = checkPoint;

        optionButton.SetActive(true);
        optionButton.GetComponent<OptionButtons>().OpsionButtonsPanelSwitching(false);

        Time.timeScale = 1;
    }
}