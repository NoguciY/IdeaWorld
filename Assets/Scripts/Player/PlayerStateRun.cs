using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerStateRun : PlayerState
{
    PlayerController _playerController;
    float speed;                    //走る速さ
    float inAirValue = 0.5f;        //空中時の走る速度にかかる係数
    float beforeKey;                //前回押したキー
    float beforeVelocityY = 0.0f;   //着地判定用
    float axisH;                    //走るキーの入力値
    bool jumpKey;                   //ジャンプキー押下の判定
    bool isJumping;                 //ジャンプ中か
    bool hitBalloon;                //風船に当たったか
    bool onGround;                  //着地判定
    bool isHittingCollider;         //コライダーに当たっているか

    //走り状態であることを示す
    public State GetState => State.Run;

    //コンストラクタ
    public PlayerStateRun(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
        //アニメーション再生
        _playerController.Animator.SetBool("isRunning", true);

        isJumping = _playerController.IsJumping;

        if (!isJumping)
        {
            //SEの再生
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

        //ジャンプ中ではない場合
        if (!isJumping)
        {
            //アニメーション停止
            _playerController.Animator.SetBool("isJumping", false);
            //走る処理
            _playerController.RunSpeed = speed * axisH;


            //ジャンプ中でない かつ 接地していない場合
            if (!onGround)
            {
                if (_playerController.Animator.GetBool("isRunning"))
                {
                    //アニメーション停止
                    _playerController.Animator.SetBool("isRunning", false);
                }
            }
            //再び地面に接地した場合
            else if (onGround)
            {
                if (!_playerController.Animator.GetBool("isRunning"))
                {
                    //アニメーション再生
                    _playerController.Animator.SetBool("isRunning", true);
                }
            }
        }
        //ジャンプ中の場合
        else
        {
            //アニメーション停止
            _playerController.Animator.SetBool("isRunning", false);

            //ジャンプアニメーションの判定
            _playerController.Animator.SetBool("isJumping", true);
            //ジャンプ時は通常時より加算する速度を減らす
            _playerController.RunSpeed = speed * axisH * inAirValue;


            //ジャンプ中から着地した時の処理
            if (onGround) //&& beforeVelocityY < 0)
            {
                _playerController.IsJumping = false;
                isJumping = false;

                //アニメーション再生
                _playerController.Animator.SetBool("isRunning", true);

                //SEの再生
                _playerController.RunningAudioSource.Play();
            }
        }

        //向きが変わった場合、一度止めて加算する速度を0にする
        if (axisH > 0 && beforeKey < 0)
        {
            _playerController.RunSpeed = 0.0f;
        }
        else if (axisH < 0 && beforeKey > 0)
        {
            _playerController.RunSpeed = 0.0f;
        }
        beforeKey = axisH;

        //--状態を変える条件--------------------------------------------------

        //ジャンプキーを押しているかつ
        //接地しているかつ
        //ジャンプ中でない場合
        if (jumpKey && onGround && !isJumping)
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

        //走るキーが押されていない場合
        if (axisH == 0)
        {
            //待機状態へ移行
            _playerController.Idle();
        }
        //--------------------------------------------------------------------

        beforeVelocityY = _playerController.Rigidb.velocity.y;
    }

    public void Exit()
    {
        //加算する速度を0にする
        _playerController.RunSpeed = 0;

        //SEの停止
        _playerController.RunningAudioSource.Stop();

        //走るのを止めた時に接地していたかつ
        //ジャンプキーが押されていない場合
        //x方向の速度を0にする
        if (onGround && !jumpKey)
        {
            _playerController.Rigidb.velocity =
            new Vector2(0, _playerController.Rigidb.velocity.y);
        }

        //風船に当たったかの判定を返す
        _playerController.HitBalloon = hitBalloon;

        //走るアニメーションの停止
        _playerController.Animator.SetBool("isRunning", false);
    }
}