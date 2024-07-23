using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�v���C���[�̏�ԑJ�ڊǗ����s���N���X
public class PlayerStateMachine
{
    PlayerState currentState;   //���݂̏��
    PlayerState preState;       //���O�̏��

    //��Ԃ̃e�[�u��
    Dictionary<State, PlayerState> stateTable;

    public void Init(PlayerController playerController, State initState)
    {
        //��������1�x����
        if (stateTable != null) return;

        //�e��ԃN���X�̏�����
        Dictionary<State, PlayerState> table = new()
        {
            { State.Idle, new PlayerStateIdle(playerController) },
            { State.Run,  new PlayerStateRun (playerController) },
            { State.Jump, new PlayerStateJump(playerController) },
            { State.Hang, new PlayerStateHang(playerController) },
        };
        stateTable = table;

        currentState = stateTable[initState];
        //������Ԃ̊J�n����
        currentState.Enter();
    }

    //�ʂ̏�ԂɕύX����֐�
    public void ChangeState(State nextState)
    {
        //������Ԃɂ͑J�ڂ��Ȃ�
        if (currentState == null ||
            currentState.GetState == nextState)
        {
            return;
        }

        //���̏�Ԃ��i�[
        var next = stateTable[nextState];
        preState = currentState;
        //�O�̏�Ԃ̏I������
        preState?.Exit();
        //���݂̏�Ԃ�ύX
        currentState = next;
        //�J�n���̏���
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
        {
            //Debug.Log("�v���C���[�̏��" + currentState);
            currentState.Update();
        }
    }
}
