using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ぶら下がり状態
 * 風船に当たるとこの状態になる
 * プレイヤーは風船の子になる
 * プレイヤーの位置を風船にくっつけるようにする
 * 移動キーで親の風船を動かす
 * 移動は走るのよりもゆっくり
 * 風船が壊れた場合、待機状態に遷移する
 */

public class PlayerStateHang : PlayerState
{
    PlayerController _playerController;
    BalloonController balloonController;
    bool isHittingCollider;
    Vector3 playerPos;
    Vector3 playerPosOffset = new Vector3(0.0f, -2.5f, 0.0f);   //プレイヤーと風船の相対距離

    //ぶら下がり状態であることを示す
    public State GetState => State.Hang;

    //コンストラクタ
    public PlayerStateHang(PlayerController playerController) => _playerController = playerController;

    public void Enter()
    {
        //アニメーション再生
        _playerController.Animator.SetBool("isHanging", true);

        _playerController.IsHanging = true;

        //風船を動かすために取得
        balloonController = _playerController.BalloonController;
    }

    public void Update()
    {
        float axisH = _playerController.AxisH;          //走るキーの入力値
        bool onGround = _playerController.OnGround;     //着地判定
        float speed = 1.0f;                             //ぶら下がり時の横方向の速度
        isHittingCollider = _playerController.IsHittingCollider;

        if (balloonController)
        {
            //ぶら下がり状態の位置
            playerPos = balloonController.transform.position + playerPosOffset;

            //風船にくっつくような位置にプレイヤーを移動させる
            _playerController.transform.position = playerPos;

            //風船の移動処理
            balloonController.speedX = speed * axisH;

            //風船の向きを変えるための処理
            balloonController.AxisH = axisH;
        }

        //--状態を変える条件--------------------------------------------------

        //風船が壊れた場合、待機状態
        if (!balloonController)
        {
            //待機状態へ移行
            _playerController.Idle();
        }

        //----------------------------------------------------------------------
    }

    public void Exit()
    {
        _playerController.IsHanging = false;

        //アニメーション再生
        _playerController.Animator.SetBool("isHanging", false);
    }
}
