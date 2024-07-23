using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerState
{
    PlayerController _playerController;
    float jumpForce;            //ジャンプ力
    float axisH;                //走るキーの入力値

    //ジャンプ状態であることを示す
    public State GetState => State.Jump;

    //コンストラクタ
    public PlayerStateJump(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
        //ジャンプ再生
        _playerController.Animator.SetTrigger("jumpTrigger");

        jumpForce = _playerController.JumpForce;

        //ジャンプの処理
        _playerController.Rigidb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        //SEの再生
        _playerController.JumpingAudioSource.Play();
    }

    public void Update()
    {
        axisH = _playerController.AxisH;

        //--状態を変える条件--------------------------------------------------
        
        //待機状態へ移行
        _playerController.Idle();

        //---------------------------------------------------------------------
    }

    public void Exit()
    {
        //ジャンプ中であることを報告
        _playerController.IsJumping = true;
    }
}
