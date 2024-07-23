using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

//�X�e�[�W�Ǘ��X�N���v�g����X�e�[�W�����擾���āA
//�J���������݂̃X�e�[�W�̈���Ńv���C���[��Ǐ]����悤�ɂ���
//�X�e�[�W�ԍ��ŃJ�����̉f���͈͂�ς�����A�v���C���[�̒Ǐ]�ʒu��ς�����

public class CameraManager : MonoBehaviour
{
    public float cameraWidth = 8.85f, cameraHeight = 5.0f;  //�J�����̕�(����)
    [SerializeField] int nowStage;                          //���݂̃X�e�[�W
    public GameObject[] rawStages;                          //�I���W�i���̃X�e�[�W�̈�
    public GameObject player;                               //�J�������Ǐ]����Ώ�

    private const int stageMax = 100;                       //�ő�X�e�[�W��

    private Vector2[] stageSizes;                           //�X�e�[�W�̗̈�
    float edgeRight, edgeLeft, edgeTop, edgeBottom;         //��ʒ[�̍��W
    float[] cameraMovementRangeChengesPosX;                 //�J�����̈ړ��͈͂��ς��x���W
    int tmpStageNum;                                        //�X�e�[�W�ԍ������ԂɊi�[����ϐ�
    GameObject[] stages;                                    //�X�e�[�W�̈�
    bool[] usingBoxColliderStages;                          //�{�b�N�X�R���C�_�[���g���Ă���X�e�[�W��
    Vector2[] secondEdgeColliderStageSizes;                 //2�ڂ̃X�e�[�W�̗̈�

    //�Z�b�^�[
    public int NowStage { set { nowStage = value; } }    

    void Start()
    {
        //�ő�X�e�[�W�����̔z���p�ӂ���
        stages = new GameObject[stageMax];
        stageSizes = new Vector2[stageMax];
        secondEdgeColliderStageSizes = new Vector2[stageMax];
        usingBoxColliderStages = new bool[stageMax];
        cameraMovementRangeChengesPosX = new float[stageMax];

        for (int i = 0; i < rawStages.Length; i++)
        {
            //�X�e�[�W�ԍ����擾
            tmpStageNum = rawStages[i].GetComponent<StageManager>().StageNum;
            
            //�X�e�[�W�����ő�X�e�[�W���ȏ�̏ꍇ�̏���
            if (tmpStageNum >= stageMax)
            {
                Debug.Log("Error! �X�e�[�W�����������܂�");
                continue;
            }

            //�X�e�[�W�����ԂɊi�[���āA�X�e�[�W�̗̈���擾����
            stages[tmpStageNum] = rawStages[i];

            //�{�b�N�X�R���C�_�[������ȊO�̃R���C�_�[(�G�b�W�R���C�_�[)���ŃX�e�[�W�̈�̎擾���@��ς���
            if (rawStages[i].GetComponent<StageManager>().UsingBoxCollider)
            {
                //�{�b�N�X�R���C�_�[���g���Ă���
                usingBoxColliderStages[tmpStageNum] = true;

                //�X�e�[�W�̈�̎擾
                stageSizes[tmpStageNum] = rawStages[i].GetComponent<BoxCollider2D>().size;

                //�g��Ȃ���0�������Ă���
                cameraMovementRangeChengesPosX[tmpStageNum] = 0.0f;
                secondEdgeColliderStageSizes[tmpStageNum] = Vector2.zero;
            }
            else 
            {
                //�{�b�N�X�R���C�_�[���g���Ă��Ȃ�
                usingBoxColliderStages[tmpStageNum] = false;

                //�J�����̈ړ��͈͂�ς���x���W���擾
                cameraMovementRangeChengesPosX[tmpStageNum] = rawStages[i].GetComponent<StageManager>().CameraMovementRangeChengesPosX;

                //�X�e�[�W�̈�̎擾
                //1��
                stageSizes[tmpStageNum] = rawStages[i].GetComponent<StageManager>().FirstEdgeColliderStageSize;
                //2��
                secondEdgeColliderStageSizes[tmpStageNum] = rawStages[i].GetComponent<StageManager>().SecondEdgeColliderStageSize;
            }
        }
    }

    void Update()
    {
        //�J�����̈ړ��̐����Ɏg��
        CalcScreenEdge();

        //Debug.Log("���[" + edgeLeft + "�E�[" + edgeRight + "���["+ edgeBottom + "��[" + edgeTop);

        // �J�����̉��������[�v�ړ�
        //�J��������ʊO���f���Ȃ��悤�ɂ���B�܂��A�v���C���[�����X�|�[�������ۂ��߂�
        if (edgeLeft >= transform.position.x || edgeLeft >= player.transform.position.x)
        {
            //�J���������[�ɖ߂�
            transform.position += (edgeLeft - transform.position.x) * Vector3.right;
        }
        else if (edgeRight <= transform.position.x)
        {
            //�J�������E�[�ɖ߂�
            transform.position += (edgeRight - transform.position.x) * Vector3.right;
        }

        // �J�����̏c�������[�v�ړ�
                                             //�v���C���[����ʂ̉��ɂ��ăJ�������ɓ����Ă��Ȃ����Ԃ�h�~
        if (edgeBottom >= transform.position.y || edgeBottom >= player.transform.position.y)
        {
            //�J���������[�ɖ߂�
            transform.position += (edgeBottom - transform.position.y) * Vector3.up;
        }
        else if (edgeTop <= transform.position.y)
        {
            //�J��������[�ɖ߂�
            transform.position += (edgeTop - transform.position.y) * Vector3.up;
        }

        //�v���C���[��x,y���W���J������������͈͓��̎��A�J�������v���C���[��Ǐ]����
        if (player.transform.position.x > edgeLeft && player.transform.position.x < edgeRight)
        {
            //�J������x���W���v���C���[��x���W�ɂ���
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
        if (player.transform.position.y > edgeBottom && player.transform.position.y < edgeTop)
        {
            //�J������y���W���v���C���[��y���W�ɂ���
            transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        }


    }

    //��ʒ[�̍��W���v�Z����
    void CalcScreenEdge()
    {
        //�{�b�N�X�R���C�_�[�̉�ʒ[�̌v�Z
        if (usingBoxColliderStages[nowStage])
        {
            //�J�������ړ��ł��鍶�[ = ���݂̃X�e�[�W��x���W - ���݂̃X�e�[�W�̔����̉��� + �J�����̉���
            edgeLeft = stages[nowStage].transform.position.x - (stageSizes[nowStage].x / 2) + cameraWidth;

            //�J�������ړ��ł���E�[ = ���݂̃X�e�[�W��x���W + ���݂̃X�e�[�W��x���W�̔��� - �J�����̉���
            edgeRight = stages[nowStage].transform.position.x + (stageSizes[nowStage].x / 2) - cameraWidth;

            //�J�������ړ��ł��鉺�[ = ���݂̃X�e�[�W��y���W + ���݂̃X�e�[�W��y���W�̔��� + �J�����̏c��
            edgeBottom = stages[nowStage].transform.position.y - (stageSizes[nowStage].y / 2) + cameraHeight;

            //�J�������ړ��ł����[ = ���݂̃X�e�[�W��y���W + ���݂̃X�e�[�W��y���W�̔��� + �J�����̏c��
            edgeTop = stages[nowStage].transform.position.y + (stageSizes[nowStage].y / 2) - cameraHeight;
        }
        //�{�b�N�X�R���C�_�[�ł͂Ȃ�(�G�b�W�R���C�_�[��)��ʒ[�̌v�Z
        else
        {
            if (stages[nowStage].transform.position.x + cameraMovementRangeChengesPosX[nowStage] > player.transform.position.x)
            {
                edgeLeft = stages[nowStage].transform.position.x + cameraWidth;
                edgeRight = stages[nowStage].transform.position.x + stageSizes[nowStage].x - cameraWidth;
                edgeBottom = stages[nowStage].transform.position.y + cameraHeight;
                edgeTop = stages[nowStage].transform.position.y + stageSizes[nowStage].y - cameraHeight;
            }
            else
            {
                edgeLeft = stages[nowStage].transform.position.x + cameraWidth;
                edgeRight = stages[nowStage].transform.position.x + stageSizes[nowStage].x - cameraWidth;
                edgeBottom = stages[nowStage].transform.position.y + cameraHeight;
                edgeTop = stages[nowStage].transform.position.y + secondEdgeColliderStageSizes[nowStage].y - cameraHeight;
                //Debug.Log(secondEdgeColliderStageSizes[nowStage]);
            }
        }
    }
}