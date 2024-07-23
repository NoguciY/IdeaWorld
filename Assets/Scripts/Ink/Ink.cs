using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ink : MonoBehaviour
{
    [Header("自然消滅までの時間")]
    public float timeLimit;


    //カウントダウンで使う
    float time;

    //Poolの参照
    [HideInInspector] public InkAndParentPool pool;

    [HideInInspector] public MarkerManager markerManager;

    //trueでカウントダウン開始
    [HideInInspector] public bool countDown = false;

    //アクティブになった時の処理
    void OnEnable()
    {
        //timeをリセット
        time = timeLimit;
        //countDownをfalseにする
        countDown = false;
    }

    void Update()
    {
        if (countDown)
        {
            //時間でInk消える
            time -= Time.deltaTime;
            if (time <= 0)
            {
                markerManager.parentQueue.Dequeue();

                CollectPool();
            }

        }
    }

    public void CollectPool()
    {
        //Inkは、レイヤーを0にして返す
        Transform[] InkObjects = gameObject.GetComponentsInChildren<Transform>();
        int count = InkObjects.Length;
        // i=1 は、親が入ってるインデックスを飛ばすため8
        for (int i = 1; i < count; i++)
        {
            //inkのレイヤーを戻して返却
            InkObjects[i].gameObject.layer = 0;
            pool.CollectInk(InkObjects[i].gameObject);
        }

        //親子関係を解除
        transform.DetachChildren();

        //parentもRigidbody２Dを外して返す
        Destroy(GetComponent<Rigidbody2D>());
        pool.CollectParent(transform);
    }
}
