using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerState
{
    PlayerController _playerController;
    float jumpForce;            //�W�����v��
    float axisH;                //����L�[�̓��͒l

    //�W�����v��Ԃł��邱�Ƃ�����
    public State GetState => State.Jump;

    //�R���X�g���N�^
    public PlayerStateJump(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
        //�W�����v�Đ�
        _playerController.Animator.SetTrigger("jumpTrigger");

        jumpForce = _playerController.JumpForce;

        //�W�����v�̏���
        _playerController.Rigidb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        //SE�̍Đ�
        _playerController.JumpingAudioSource.Play();
    }

    public void Update()
    {
        axisH = _playerController.AxisH;

        //--��Ԃ�ς������--------------------------------------------------
        
        //�ҋ@��Ԃֈڍs
        _playerController.Idle();

        //---------------------------------------------------------------------
    }

    public void Exit()
    {
        //�W�����v���ł��邱�Ƃ��
        _playerController.IsJumping = true;
    }
}
