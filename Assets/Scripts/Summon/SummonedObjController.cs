using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������̑�̂̃p�����[�^�[��
//�������œ����֐����܂Ƃ߂�Ƃ���

public class SummonedObjController : MonoBehaviour
{
    [System.NonSerialized] public Vector3 pos;          //�������̈ʒu
    [System.NonSerialized] public float lifeTime;       //�������̐�������
    public float lifeSpan = 10.0f;                      //�������̎���
    public float speed;                                 //�ړ����x

    // Update is called once per frame
    protected virtual void Update()
    {
        lifeTime += Time.deltaTime;
    }

    //�������̔\�͂��L�q����֐�
    protected virtual void SummonedObjSkill()
    {

    }

    //�����������ł�����֐�
    protected virtual void DestroySummonedObj()
    {

    }
}
