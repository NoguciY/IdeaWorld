using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

//�v���C���[�̏�Ԃ̊��N���X

//�v���C���[�̏��
public enum State
{
    Idle,   //�ҋ@
    Run,    //����
    Jump,   //�W�����v
    Hang,   //�Ԃ牺����
}

public interface PlayerState
{
    //���̃N���X�̏�Ԃ��擾����
    public State GetState { get; }

    /// <summary>
    /// ��ԊJ�n���Ɏ��s����
    /// </summary>
    void Enter();

    /// <summary>
    /// ���t���[�����s����
    /// </summary>
    void Update();

    /// <summary>
    /// ��ԏI�����Ɏ��s����
    /// </summary>
    void Exit();
}
