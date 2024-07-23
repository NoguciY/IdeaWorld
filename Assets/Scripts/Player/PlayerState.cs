using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

//プレイヤーの状態の基底クラス

//プレイヤーの状態
public enum State
{
    Idle,   //待機
    Run,    //走る
    Jump,   //ジャンプ
    Hang,   //ぶら下がる
}

public interface PlayerState
{
    //このクラスの状態を取得する
    public State GetState { get; }

    /// <summary>
    /// 状態開始時に実行する
    /// </summary>
    void Enter();

    /// <summary>
    /// 毎フレーム実行する
    /// </summary>
    void Update();

    /// <summary>
    /// 状態終了時に実行する
    /// </summary>
    void Exit();
}
