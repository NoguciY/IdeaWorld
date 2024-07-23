using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

//ステージ管理スクリプトからステージ情報を取得して、
//カメラが現在のステージ領域内でプレイヤーを追従するようにする
//ステージ番号でカメラの映す範囲を変えたり、プレイヤーの追従位置を変えたい

public class CameraManager : MonoBehaviour
{
    public float cameraWidth = 8.85f, cameraHeight = 5.0f;  //カメラの幅(半分)
    [SerializeField] int nowStage;                          //現在のステージ
    public GameObject[] rawStages;                          //オリジナルのステージ領域
    public GameObject player;                               //カメラが追従する対象

    private const int stageMax = 100;                       //最大ステージ数

    private Vector2[] stageSizes;                           //ステージの領域
    float edgeRight, edgeLeft, edgeTop, edgeBottom;         //画面端の座標
    float[] cameraMovementRangeChengesPosX;                 //カメラの移動範囲が変わるx座標
    int tmpStageNum;                                        //ステージ番号を順番に格納する変数
    GameObject[] stages;                                    //ステージ領域
    bool[] usingBoxColliderStages;                          //ボックスコライダーを使っているステージか
    Vector2[] secondEdgeColliderStageSizes;                 //2つ目のステージの領域

    //セッター
    public int NowStage { set { nowStage = value; } }    

    void Start()
    {
        //最大ステージ数分の配列を用意する
        stages = new GameObject[stageMax];
        stageSizes = new Vector2[stageMax];
        secondEdgeColliderStageSizes = new Vector2[stageMax];
        usingBoxColliderStages = new bool[stageMax];
        cameraMovementRangeChengesPosX = new float[stageMax];

        for (int i = 0; i < rawStages.Length; i++)
        {
            //ステージ番号を取得
            tmpStageNum = rawStages[i].GetComponent<StageManager>().StageNum;
            
            //ステージ数が最大ステージ数以上の場合の処理
            if (tmpStageNum >= stageMax)
            {
                Debug.Log("Error! ステージ数が多すぎます");
                continue;
            }

            //ステージを順番に格納して、ステージの領域を取得する
            stages[tmpStageNum] = rawStages[i];

            //ボックスコライダーかそれ以外のコライダー(エッジコライダー)かでステージ領域の取得方法を変える
            if (rawStages[i].GetComponent<StageManager>().UsingBoxCollider)
            {
                //ボックスコライダーを使っている
                usingBoxColliderStages[tmpStageNum] = true;

                //ステージ領域の取得
                stageSizes[tmpStageNum] = rawStages[i].GetComponent<BoxCollider2D>().size;

                //使わないが0を代入しておく
                cameraMovementRangeChengesPosX[tmpStageNum] = 0.0f;
                secondEdgeColliderStageSizes[tmpStageNum] = Vector2.zero;
            }
            else 
            {
                //ボックスコライダーを使っていない
                usingBoxColliderStages[tmpStageNum] = false;

                //カメラの移動範囲を変えるx座標を取得
                cameraMovementRangeChengesPosX[tmpStageNum] = rawStages[i].GetComponent<StageManager>().CameraMovementRangeChengesPosX;

                //ステージ領域の取得
                //1つ目
                stageSizes[tmpStageNum] = rawStages[i].GetComponent<StageManager>().FirstEdgeColliderStageSize;
                //2つ目
                secondEdgeColliderStageSizes[tmpStageNum] = rawStages[i].GetComponent<StageManager>().SecondEdgeColliderStageSize;
            }
        }
    }

    void Update()
    {
        //カメラの移動の制限に使う
        CalcScreenEdge();

        //Debug.Log("左端" + edgeLeft + "右端" + edgeRight + "下端"+ edgeBottom + "上端" + edgeTop);

        // カメラの横方向ワープ移動
        //カメラが画面外を映さないようにする。また、プレイヤーがリスポーンした際も戻す
        if (edgeLeft >= transform.position.x || edgeLeft >= player.transform.position.x)
        {
            //カメラを左端に戻す
            transform.position += (edgeLeft - transform.position.x) * Vector3.right;
        }
        else if (edgeRight <= transform.position.x)
        {
            //カメラを右端に戻す
            transform.position += (edgeRight - transform.position.x) * Vector3.right;
        }

        // カメラの縦方向ワープ移動
                                             //プレイヤーが画面の下にいてカメラ内に入っていない事態を防止
        if (edgeBottom >= transform.position.y || edgeBottom >= player.transform.position.y)
        {
            //カメラを下端に戻す
            transform.position += (edgeBottom - transform.position.y) * Vector3.up;
        }
        else if (edgeTop <= transform.position.y)
        {
            //カメラを上端に戻す
            transform.position += (edgeTop - transform.position.y) * Vector3.up;
        }

        //プレイヤーのx,y座標がカメラが動ける範囲内の時、カメラがプレイヤーを追従する
        if (player.transform.position.x > edgeLeft && player.transform.position.x < edgeRight)
        {
            //カメラのx座標をプレイヤーのx座標にする
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
        if (player.transform.position.y > edgeBottom && player.transform.position.y < edgeTop)
        {
            //カメラのy座標をプレイヤーのy座標にする
            transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        }


    }

    //画面端の座標を計算する
    void CalcScreenEdge()
    {
        //ボックスコライダーの画面端の計算
        if (usingBoxColliderStages[nowStage])
        {
            //カメラが移動できる左端 = 現在のステージのx座標 - 現在のステージの半分の横幅 + カメラの横幅
            edgeLeft = stages[nowStage].transform.position.x - (stageSizes[nowStage].x / 2) + cameraWidth;

            //カメラが移動できる右端 = 現在のステージのx座標 + 現在のステージのx座標の半分 - カメラの横幅
            edgeRight = stages[nowStage].transform.position.x + (stageSizes[nowStage].x / 2) - cameraWidth;

            //カメラが移動できる下端 = 現在のステージのy座標 + 現在のステージのy座標の半分 + カメラの縦幅
            edgeBottom = stages[nowStage].transform.position.y - (stageSizes[nowStage].y / 2) + cameraHeight;

            //カメラが移動できる上端 = 現在のステージのy座標 + 現在のステージのy座標の半分 + カメラの縦幅
            edgeTop = stages[nowStage].transform.position.y + (stageSizes[nowStage].y / 2) - cameraHeight;
        }
        //ボックスコライダーではない(エッジコライダーの)画面端の計算
        else
        {
            if (stages[nowStage].transform.position.x + cameraMovementRangeChengesPosX[nowStage] > player.transform.position.x)
            {
                edgeLeft = stages[nowStage].transform.position.x + cameraWidth;
                edgeRight = stages[nowStage].transform.position.x + stageSizes[nowStage].x - cameraWidth;
                edgeBottom = stages[nowStage].transform.position.y + cameraHeight;
                edgeTop = stages[nowStage].transform.position.y + stageSizes[nowStage].y - cameraHeight;
            }
            else
            {
                edgeLeft = stages[nowStage].transform.position.x + cameraWidth;
                edgeRight = stages[nowStage].transform.position.x + stageSizes[nowStage].x - cameraWidth;
                edgeBottom = stages[nowStage].transform.position.y + cameraHeight;
                edgeTop = stages[nowStage].transform.position.y + secondEdgeColliderStageSizes[nowStage].y - cameraHeight;
                //Debug.Log(secondEdgeColliderStageSizes[nowStage]);
            }
        }
    }
}