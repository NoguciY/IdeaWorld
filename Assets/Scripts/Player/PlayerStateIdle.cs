using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : PlayerState
{
    PlayerController _playerController;
    bool onGround;                  //着地判定
    float axisH;                    //走るキーの入力値
    float beforeVelocityY = 0.0f;   //着地判定用
    bool jumpKey;                   //ジャンプキー押下の判定
    bool isJumping;                 //ジャンプ中か
    bool hitBalloon;                //風船に当たったか

    //待機状態であることを示す
    public State GetState => State.Idle;

    //コンストラクタ
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
        
        //ジャンプ中の場合
        if (isJumping)
        {
            _playerController.Animator.SetBool("isJumping", true);

            //前回のy方向の速度がマイナスでかつ接地した場合
            if (onGround && beforeVelocityY < 0)
            {
                //プレイヤーを止める
                _playerController.Rigidb.velocity =
                new Vector2(0, _playerController.Rigidb.velocity.y);
                
                //ジャンプ中ではない
                isJumping = false;
                _playerController.Animator.SetBool("isJumping", false);
            }
        }

        //--状態を変える条件--------------------------------------------------
        //走るキーが押されいる場合
        if (axisH != 0)
        {
            //走り状態へ移行
            _playerController.Run();
        }

        //ジャンプキーを押しているかつ
        //接地している場合
        if (jumpKey && onGround)
        {
            //ジャンプ状態へ移行
            _playerController.Jump();
        }

        //風船に当たった場合
        if (hitBalloon)
        {
            hitBalloon = false;
            //ぶら下がり状態へ移行
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
