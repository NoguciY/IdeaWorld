using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//召喚物を生成するクラス

public class SummonedObjGenerator : MonoBehaviour
{
    public GameObject wolf;     //生成するオブジェクト(狼)
    public GameObject balloon;  //生成するオブジェクト(風船)
    public GameObject player;   //プレイヤー

    Vector2 spawnPosWolf;       //狼の生成位置
    Vector2 spawnPosBalloon;    //風船の生成位置

    //障害物レイヤーの指定
    LayerMask layerMask = (1 << 3) | (1 << 6) | (1 << 7);

    Vector2 summonedPosition;  //オブジェクトの生成位置

    //召喚物を生成できるか判定する関数
    public bool CheckSummoningSpace(int index)
    {

        Collider2D hit = null;
        Vector2 size = Vector2.zero;
        //switchで召喚獣ごとの召喚スペースの半径や場所を変数にいれて、switch文を抜けたあとの、Rayで確認する処理を描く予定
        switch (index)
        {

            case 0://風船

                summonedPosition = new Vector2(player.transform.position.x, player.transform.position.y + 4);

                size = new Vector2(2f, 1.5f);


                //スケッチモードに行く前に、召喚できるスペースがあるかの確認
                hit = Physics2D.OverlapCapsule(summonedPosition, size, CapsuleDirection2D.Vertical,0, layerMask, -1f, 1f);

                break;

            case 1://狼

                //プレイヤーの座標を取得
                Vector2 playerPos = player.transform.position;

                if (player.transform.localScale.x > 0)
                {
                    //右＋プレイヤーの中心座標が足元だからその分上に生成する
                    summonedPosition = playerPos + Vector2.right * 2 + new Vector2(0, 1.5f);
                }
                //プレイヤーが左向きの場合
                else
                {
                    //右＋プレイヤーの中心座標が足元だからその分上に生成する
                    summonedPosition = playerPos + Vector2.left * 2 + new Vector2(0, 1.5f);
                }

                size = new Vector2(2f, 1.5f);

                //スケッチモードに行く前に、召喚できるスペースがあるかの確認
                hit = Physics2D.OverlapCapsule(summonedPosition, size, CapsuleDirection2D.Vertical, 0, layerMask, -10f, 10f);

                break;
        }

        //障害物がある場合は召喚できない
        if (hit != null)
        {     
            Debug.Log(hit.name);
            return false;
        }

        return true;

    }

    //召喚物を生成する関数
    public void SummonObj(int index)
    {
        Vector2 playerPos;
        switch (index)
        {

            case 0: //風船

                //プレイヤーの座標を取得
                playerPos = player.transform.position;
                //プレイヤーの上にスポーン
                Vector2 spawnTopPos = new Vector2(player.transform.position.x, player.transform.position.y + 3);

                //召喚物を生成する
                Instantiate(balloon, spawnTopPos, Quaternion.identity);
                break;

            case 1: //狼

                //プレイヤーの座標を取得
                playerPos = player.transform.position;

                //プレイヤーが右向きの場合
                if (player.transform.localScale.x > 0)
                {
                    //右に生成する
                    spawnPosWolf = playerPos + Vector2.right * 2 + new Vector2(0, 0.1f);
                }
                //プレイヤーが左向きの場合
                else
                {
                    //左に生成する
                    spawnPosWolf = playerPos + Vector2.left * 2 + new Vector2(0, 0.1f);
                }
                //プレイヤーの前２mにスポーン
                Vector2 spawnFrontPos = spawnPosWolf;

                //召喚物を生成する
                Instantiate(wolf, spawnFrontPos, Quaternion.identity);
                break;
        }
    }
}
