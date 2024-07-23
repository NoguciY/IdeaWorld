using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの状態遷移管理を行うクラス
public class PlayerStateMachine
{
    PlayerState currentState;   //現在の状態
    PlayerState preState;       //直前の状態

    //状態のテーブル
    Dictionary<State, PlayerState> stateTable;

    public void Init(PlayerController playerController, State initState)
    {
        //初期化は1度だけ
        if (stateTable != null) return;

        //各状態クラスの初期化
        Dictionary<State, PlayerState> table = new()
        {
            { State.Idle, new PlayerStateIdle(playerController) },
            { State.Run,  new PlayerStateRun (playerController) },
            { State.Jump, new PlayerStateJump(playerController) },
            { State.Hang, new PlayerStateHang(playerController) },
        };
        stateTable = table;

        currentState = stateTable[initState];
        //初期状態の開始処理
        currentState.Enter();
    }

    //別の状態に変更する関数
    public void ChangeState(State nextState)
    {
        //同じ状態には遷移しない
        if (currentState == null ||
            currentState.GetState == nextState)
        {
            return;
        }

        //次の状態を格納
        var next = stateTable[nextState];
        preState = currentState;
        //前の状態の終了処理
        preState?.Exit();
        //現在の状態を変更
        currentState = next;
        //開始時の処理
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
        {
            //Debug.Log("プレイヤーの状態" + currentState);
            currentState.Update();
        }
    }
}
