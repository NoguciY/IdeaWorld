using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �Ԃ牺������
 * ���D�ɓ�����Ƃ��̏�ԂɂȂ�
 * �v���C���[�͕��D�̎q�ɂȂ�
 * �v���C���[�̈ʒu�𕗑D�ɂ�������悤�ɂ���
 * �ړ��L�[�Őe�̕��D�𓮂���
 * �ړ��͑���̂����������
 * ���D����ꂽ�ꍇ�A�ҋ@��ԂɑJ�ڂ���
 */

public class PlayerStateHang : PlayerState
{
    PlayerController _playerController;
    BalloonController balloonController;
    bool isHittingCollider;
    Vector3 playerPos;
    Vector3 playerPosOffset = new Vector3(0.0f, -2.5f, 0.0f);   //�v���C���[�ƕ��D�̑��΋���

    //�Ԃ牺�����Ԃł��邱�Ƃ�����
    public State GetState => State.Hang;

    //�R���X�g���N�^
    public PlayerStateHang(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
        //�A�j���[�V�����Đ�
        _playerController.Animator.SetBool("isHanging", true);

        _playerController.IsHanging = true;

        //���D�𓮂������߂Ɏ擾
        balloonController = _playerController.BalloonController;
    }

    public void Update()
    {
        float axisH = _playerController.AxisH;          //����L�[�̓��͒l
        bool onGround = _playerController.OnGround;     //���n����
        float speed = 1.0f;                             //�Ԃ牺���莞�̉������̑��x
        isHittingCollider = _playerController.IsHittingCollider;

        if (balloonController)
        {
            //�Ԃ牺�����Ԃ̈ʒu
            playerPos = balloonController.transform.position + playerPosOffset;

            //���D�ɂ������悤�Ȉʒu�Ƀv���C���[���ړ�������
            _playerController.transform.position = playerPos;

            //���D�̈ړ�����
            balloonController.speedX = speed * axisH;

            //���D�̌�����ς��邽�߂̏���
            balloonController.AxisH = axisH;
        }

        //--��Ԃ�ς������--------------------------------------------------

        //���D����ꂽ�ꍇ�A�ҋ@���
        if (!balloonController)
        {
            //�ҋ@��Ԃֈڍs
            _playerController.Idle();
        }

        //----------------------------------------------------------------------
    }

    public void Exit()
    {
        _playerController.IsHanging = false;

        //�A�j���[�V�����Đ�
        _playerController.Animator.SetBool("isHanging", false);
    }
}
