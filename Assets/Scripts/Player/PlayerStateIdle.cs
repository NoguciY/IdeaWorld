using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : PlayerState
{
    PlayerController _playerController;
    bool onGround;                  //���n����
    float axisH;                    //����L�[�̓��͒l
    float beforeVelocityY = 0.0f;   //���n����p
    bool jumpKey;                   //�W�����v�L�[�����̔���
    bool isJumping;                 //�W�����v����
    bool hitBalloon;                //���D�ɓ���������

    //�ҋ@��Ԃł��邱�Ƃ�����
    public State GetState => State.Idle;

    //�R���X�g���N�^
    public PlayerStateIdle(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
    }

    public void Update()
    {
        axisH = _playerController.AxisH;
        onGround = _playerController.OnGround;
        jumpKey = _playerController.JumpKey;
        isJumping = _playerController.IsJumping;
        hitBalloon = _playerController.HitBalloon;
        
        //�W�����v���̏ꍇ
        if (isJumping)
        {
            _playerController.Animator.SetBool("isJumping", true);

            //�O���y�����̑��x���}�C�i�X�ł��ڒn�����ꍇ
            if (onGround && beforeVelocityY < 0)
            {
                //�v���C���[���~�߂�
                _playerController.Rigidb.velocity =
                new Vector2(0, _playerController.Rigidb.velocity.y);
                
                //�W�����v���ł͂Ȃ�
                isJumping = false;
                _playerController.Animator.SetBool("isJumping", false);
            }
        }

        //--��Ԃ�ς������--------------------------------------------------
        //����L�[�������ꂢ��ꍇ
        if (axisH != 0)
        {
            //�����Ԃֈڍs
            _playerController.Run();
        }

        //�W�����v�L�[�������Ă��邩��
        //�ڒn���Ă���ꍇ
        if (jumpKey && onGround)
        {
            //�W�����v��Ԃֈڍs
            _playerController.Jump();
        }

        //���D�ɓ��������ꍇ
        if (hitBalloon)
        {
            hitBalloon = false;
            //�Ԃ牺�����Ԃֈڍs
            _playerController.Hang();
        }
        //----------------------------------------------------------------------

        beforeVelocityY = _playerController.Rigidb.velocity.y;
    }

    public void Exit()
    {
        _playerController.IsJumping = isJumping;
        _playerController.HitBalloon = hitBalloon;
    }
}
