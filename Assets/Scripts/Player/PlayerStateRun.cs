using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerStateRun : PlayerState
{
    PlayerController _playerController;
    float speed;                    //���鑬��
    float inAirValue = 0.5f;        //�󒆎��̑��鑬�x�ɂ�����W��
    float beforeKey;                //�O�񉟂����L�[
    float beforeVelocityY = 0.0f;   //���n����p
    float axisH;                    //����L�[�̓��͒l
    bool jumpKey;                   //�W�����v�L�[�����̔���
    bool isJumping;                 //�W�����v����
    bool hitBalloon;                //���D�ɓ���������
    bool onGround;                  //���n����
    bool isHittingCollider;         //�R���C�_�[�ɓ������Ă��邩

    //�����Ԃł��邱�Ƃ�����
    public State GetState => State.Run;

    //�R���X�g���N�^
    public PlayerStateRun(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
        //�A�j���[�V�����Đ�
        _playerController.Animator.SetBool("isRunning", true);

        isJumping = _playerController.IsJumping;

        if (!isJumping)
        {
            //SE�̍Đ�
            _playerController.RunningAudioSource.Play();
        }

        speed = _playerController.MoveSpeed;
    }

    public void Update()
    {
        axisH = _playerController.AxisH;
        onGround = _playerController.OnGround;
        jumpKey = _playerController.JumpKey;
        isJumping = _playerController.IsJumping;
        hitBalloon = _playerController.HitBalloon;

        //�W�����v���ł͂Ȃ��ꍇ
        if (!isJumping)
        {
            //�A�j���[�V������~
            _playerController.Animator.SetBool("isJumping", false);
            //���鏈��
            _playerController.RunSpeed = speed * axisH;


            //�W�����v���łȂ� ���� �ڒn���Ă��Ȃ��ꍇ
            if (!onGround)
            {
                if (_playerController.Animator.GetBool("isRunning"))
                {
                    //�A�j���[�V������~
                    _playerController.Animator.SetBool("isRunning", false);
                }
            }
            //�Ăђn�ʂɐڒn�����ꍇ
            else if (onGround)
            {
                if (!_playerController.Animator.GetBool("isRunning"))
                {
                    //�A�j���[�V�����Đ�
                    _playerController.Animator.SetBool("isRunning", true);
                }
            }
        }
        //�W�����v���̏ꍇ
        else
        {
            //�A�j���[�V������~
            _playerController.Animator.SetBool("isRunning", false);

            //�W�����v�A�j���[�V�����̔���
            _playerController.Animator.SetBool("isJumping", true);
            //�W�����v���͒ʏ펞�����Z���鑬�x�����炷
            _playerController.RunSpeed = speed * axisH * inAirValue;


            //�W�����v�����璅�n�������̏���
            if (onGround) //&& beforeVelocityY < 0)
            {
                _playerController.IsJumping = false;
                isJumping = false;

                //�A�j���[�V�����Đ�
                _playerController.Animator.SetBool("isRunning", true);

                //SE�̍Đ�
                _playerController.RunningAudioSource.Play();
            }
        }

        //�������ς�����ꍇ�A��x�~�߂ĉ��Z���鑬�x��0�ɂ���
        if (axisH > 0 && beforeKey < 0)
        {
            _playerController.RunSpeed = 0.0f;
        }
        else if (axisH < 0 && beforeKey > 0)
        {
            _playerController.RunSpeed = 0.0f;
        }
        beforeKey = axisH;

        //--��Ԃ�ς������--------------------------------------------------

        //�W�����v�L�[�������Ă��邩��
        //�ڒn���Ă��邩��
        //�W�����v���łȂ��ꍇ
        if (jumpKey && onGround && !isJumping)
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

        //����L�[��������Ă��Ȃ��ꍇ
        if (axisH == 0)
        {
            //�ҋ@��Ԃֈڍs
            _playerController.Idle();
        }
        //--------------------------------------------------------------------

        beforeVelocityY = _playerController.Rigidb.velocity.y;
    }

    public void Exit()
    {
        //���Z���鑬�x��0�ɂ���
        _playerController.RunSpeed = 0;

        //SE�̒�~
        _playerController.RunningAudioSource.Stop();

        //����̂��~�߂����ɐڒn���Ă�������
        //�W�����v�L�[��������Ă��Ȃ��ꍇ
        //x�����̑��x��0�ɂ���
        if (onGround && !jumpKey)
        {
            _playerController.Rigidb.velocity =
            new Vector2(0, _playerController.Rigidb.velocity.y);
        }

        //���D�ɓ����������̔����Ԃ�
        _playerController.HitBalloon = hitBalloon;

        //����A�j���[�V�����̒�~
        _playerController.Animator.SetBool("isRunning", false);
    }
}