using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//メインカメラを管理するクラス

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public GameObject subScreen;    //背景の手前に設置するサブ背景
    public float leftLimit = 0;     //左スクロールの上限
    public float rightLimit = 0;    //右スクロールの上限
    public float topLimit = 0;      //上スクロールの上限
    public float bottomLimit = 0;   //下スクロールの上限

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが存在する場合
        if (player != null)
        {
            //カメラの座標更新
            float x = player.transform.position.x;
            float y = player.transform.position.y + 3.0f;
            float z = transform.position.z;

            //横同期させる
            //上下に移動制限を付ける
            if (x < leftLimit)
            {
                x = leftLimit;
            }
            else if (x > rightLimit)
            {
                x = rightLimit;
            }

            //縦同期させる
            //上下に移動制限を付ける
            if (y < bottomLimit)
            {
                y = bottomLimit;
            }
            else if (y > topLimit)
            {
                y = topLimit;
            }

            //カメラ位置のVector3を作る
            Vector3 v3 = new Vector3(x, y, z);
            transform.position = v3;

            //サブスクリーンスクロール
            if (subScreen != null)
            {
                y = subScreen.transform.position.y;
                z = subScreen.transform.position.z;
                Vector3 v = new Vector3(x / 2.0f, y, z);
                subScreen.transform.position = v;
            }
        }
    }
}
