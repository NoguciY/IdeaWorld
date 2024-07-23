using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������𐶐�����N���X

public class SummonedObjGenerator : MonoBehaviour
{
    public GameObject wolf;     //��������I�u�W�F�N�g(�T)
    public GameObject balloon;  //��������I�u�W�F�N�g(���D)
    public GameObject player;   //�v���C���[

    Vector2 spawnPosWolf;       //�T�̐����ʒu
    Vector2 spawnPosBalloon;    //���D�̐����ʒu

    //��Q�����C���[�̎w��
    LayerMask layerMask = (1 << 3) | (1 << 6) | (1 << 7);

    Vector2 summonedPosition;  //�I�u�W�F�N�g�̐����ʒu

    //�������𐶐��ł��邩���肷��֐�
    public bool CheckSummoningSpace(int index)
    {

        Collider2D hit = null;
        Vector2 size = Vector2.zero;
        //switch�ŏ����b���Ƃ̏����X�y�[�X�̔��a��ꏊ��ϐ��ɂ���āAswitch���𔲂������Ƃ́ARay�Ŋm�F���鏈����`���\��
        switch (index)
        {

            case 0://���D

                summonedPosition = new Vector2(player.transform.position.x, player.transform.position.y + 4);

                size = new Vector2(2f, 1.5f);


                //�X�P�b�`���[�h�ɍs���O�ɁA�����ł���X�y�[�X�����邩�̊m�F
                hit = Physics2D.OverlapCapsule(summonedPosition, size, CapsuleDirection2D.Vertical,0, layerMask, -1f, 1f);

                break;

            case 1://�T

                //�v���C���[�̍��W���擾
                Vector2 playerPos = player.transform.position;

                if (player.transform.localScale.x > 0)
                {
                    //�E�{�v���C���[�̒��S���W�����������炻�̕���ɐ�������
                    summonedPosition = playerPos + Vector2.right * 2 + new Vector2(0, 1.5f);
                }
                //�v���C���[���������̏ꍇ
                else
                {
                    //�E�{�v���C���[�̒��S���W�����������炻�̕���ɐ�������
                    summonedPosition = playerPos + Vector2.left * 2 + new Vector2(0, 1.5f);
                }

                size = new Vector2(2f, 1.5f);

                //�X�P�b�`���[�h�ɍs���O�ɁA�����ł���X�y�[�X�����邩�̊m�F
                hit = Physics2D.OverlapCapsule(summonedPosition, size, CapsuleDirection2D.Vertical, 0, layerMask, -10f, 10f);

                break;
        }

        //��Q��������ꍇ�͏����ł��Ȃ�
        if (hit != null)
        {     
            Debug.Log(hit.name);
            return false;
        }

        return true;

    }

    //�������𐶐�����֐�
    public void SummonObj(int index)
    {
        Vector2 playerPos;
        switch (index)
        {

            case 0: //���D

                //�v���C���[�̍��W���擾
                playerPos = player.transform.position;
                //�v���C���[�̏�ɃX�|�[��
                Vector2 spawnTopPos = new Vector2(player.transform.position.x, player.transform.position.y + 3);

                //�������𐶐�����
                Instantiate(balloon, spawnTopPos, Quaternion.identity);
                break;

            case 1: //�T

                //�v���C���[�̍��W���擾
                playerPos = player.transform.position;

                //�v���C���[���E�����̏ꍇ
                if (player.transform.localScale.x > 0)
                {
                    //�E�ɐ�������
                    spawnPosWolf = playerPos + Vector2.right * 2 + new Vector2(0, 0.1f);
                }
                //�v���C���[���������̏ꍇ
                else
                {
                    //���ɐ�������
                    spawnPosWolf = playerPos + Vector2.left * 2 + new Vector2(0, 0.1f);
                }
                //�v���C���[�̑O�Qm�ɃX�|�[��
                Vector2 spawnFrontPos = spawnPosWolf;

                //�������𐶐�����
                Instantiate(wolf, spawnFrontPos, Quaternion.identity);
                break;
        }
    }
}
