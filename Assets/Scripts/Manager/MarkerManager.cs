using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{

    [Header("InkとParentを貸し出すプールの参照")]
    [SerializeField] InkAndParentPool pool;

    [Header("Inkの生成間隔")]
    [SerializeField] float inkSpace;

    [Header("Ink一つ当たりの重さ")]
    [SerializeField] float inkMass;

    [Header("筆の位置が壁じゃないかの円判定の大きさ (半径を入力)")]
    [SerializeField] float radius;

    [Header("書き終わった後のInkレイヤー設定")]
    public int layer;

    //オプションボタンを押した時にインクが描かれてしまうのを防ぐために、ボタン下に壁判定をつけておく
    [Header("オプションボタンの下パネル"), SerializeField] GameObject panel;

    [Header("参照関係")]
    [SerializeField]PlayerController playerController;
    [SerializeField]Transform mainCamera;

    //[Header("描いているときのSE"), SerializeField] AudioClip paintSE;

    [SerializeField] AudioSource paintAudio;
    //線が描ける時はtrue
    bool paint = false;

    //ひとつ前のマウスposition
    Vector3 startPosition;
    //新しいマウスposition
    Vector3 endPosition;


    //この親の子階層にInkを置いていく
    Transform parent;

    //現在描いている線のinkの参照を保存しておく
    Queue<GameObject> inkQueue;

    //現在の描いている線のparent参照を保存
    public Queue<Transform> parentQueue;

    //障害物レイヤーの指定
    LayerMask layerMask = (1 << 3) | (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9);



    //Update内で使用する変数
    //割合の最大( 1 =１００% )
    const int MaxRate = 1;
    float rate;
    float t;
    Vector3 distance;


    //CheckWall関数の当たり判定で使う
    Collider2D hit;


    private void Start()
    {
        //インスタンス生成
        inkQueue = new Queue<GameObject>();
        parentQueue = new Queue<Transform>();

        //InkSpace値の確認
        if (inkSpace <= 0)
        {
            Debug.Log("inkSpaceの値が0以下なので1に設定します");
            inkSpace = 1;
            return;
        }
    }


    void LateUpdate()
    {
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("InkManagerのQueueの数＝" + parentQueue.Count);
        }



        //時間停止中は線を描けない
        if (Time.timeScale == 0) return;



        //最初押した場所に親を置き、親の子としてInkを配置
        if (Input.GetMouseButtonDown(0))
        {
            paintAudio.Play();

            //プレイヤーの動きを止める
            playerController.playerMove = false;

            //線を描けるようになる(これがないと、マーカーが引けない状態の時にマウスを長押しした状態でマーカーが引ける状態に戻ってきたときバグる)
            paint = true;


            //今のマウスpositionを記録
            startPosition = new Vector3(transform.position.x,transform.position.y,0f);



            //startPositionに描くことができるか確認する
            if (CheckWall(startPosition) || ChaekDrawingRange())
            {
                //描くのを終了する
                PaintFinish();
                Debug.Log("壁に当たった:  "+1);
                return;
            }


            //記録した場所に、親を設置
            parent = pool.LendParent(startPosition);


            //Parentを借りれなかったら、一番古いのを返却して新しいのを借りる
            if (parent == null)
            {
                //古いのを取り出して、返却
                Transform parentQ = parentQueue.Dequeue();
                parentQ.GetComponent<Ink>().CollectPool();

                //返却後借りる
                parent = pool.LendParent(startPosition);

            }

            parent.GetComponent<Ink>().markerManager = this;

            //親のPositionにInkを子オブジェクトとして設置
            GameObject inkCheck = pool.LendInk(parent, startPosition);



            //Inkを借りれなかったら、親を返却して描くのを終了する
            if (inkCheck == null)
            {
                pool.CollectParent(parent);
                parent = null;
                PaintFinish();
                Debug.Log("Inkが足りない:   "+2);

                return;
            }


            //フィールドの親を管理するためにQueueに保存しておく
            parentQueue.Enqueue(parent);


            inkQueue.Enqueue(inkCheck);
        }


        //マウスの離した時の処理
        if (Input.GetMouseButtonUp(0) && paint)
        {

            //optionButtonのパネル
            panel.SetActive(true);
            PaintFinish();
        }


        //マウスを押してる間の処理
        if (Input.GetMouseButton(0) && paint)
        {
            panel.SetActive(false);

            //押してる間のマウスpositionを記録
            endPosition = new Vector3(transform.position.x, transform.position.y, 0f);


            //startからendまでの距離をInk間隔で割って、割合をだす
            rate = inkSpace / Vector3.Distance(startPosition, endPosition);


            //マウスの移動距離がInkSpace以下の場合はreturn
            if (rate >= MaxRate)
            {
                //Debug.Log("点:   "+3);

                return;
            }


            //割合を代入
            t = rate;


            while (t <= MaxRate)
            {
                //startからendまでの距離を、割合で計算
                distance = Vector3.Lerp(startPosition, endPosition, t);


                //描けるか確認
                if (CheckWall(distance) || ChaekDrawingRange())
                {

                    //描くのを終了する
                    PaintFinish();
                    Debug.Log("壁:   "+4);

                    return;
                }


                //位置を距離の割合で求めた座標にInkを生成する
                GameObject Check = pool.LendInk(parent, distance);

                //インクが足りなかったら終了
                if (Check == null)
                {
                    PaintFinish();
                    Debug.Log("Inkなし:   "+5);

                    return;
                }
                inkQueue.Enqueue(Check);


                //割合を増やす
                t += rate;

            }

            //endまでいった座標をstartに設定
            startPosition = distance;

        }
    }


    //障害物の上に描けないように確認する
    bool CheckWall(Vector3 position)
    {

        //引数のpositionにサークル状のRayでオブジェクトを検知
        hit = Physics2D.OverlapCircle(position, radius, layerMask);

        if (hit == null)
        {
            return false;
        }
        return true;
    }


    //画面内かの確認
    bool ChaekDrawingRange()
    {
        float drawingRange_right = mainCamera.position.x + 9;
        float drawingRange_left = mainCamera.position.x - 9;

        float drawingRange_top = mainCamera.position.y + 5;
        float drawingRange_bottom = mainCamera.position.y - 5;

        if (transform.position.x >= drawingRange_left && transform.position.x <= drawingRange_right && transform.position.y >= drawingRange_bottom && transform.position.y <= drawingRange_top)
        {
            return false;
        }

        return true;
    }


    //ステージ内の線をすべて消す
    public void RemoveAllLines()
    {
        while(parentQueue.Count>0)
        {
            Transform tr=parentQueue.Dequeue();
            tr.GetComponent<Ink>().CollectPool();
        }
    }


    //描くのをやめて、parentにRigidbody2Dを追加とInkのレイヤーの変更
    public void PaintFinish()
    {
        //SEのループ再生を止める
        paintAudio.Stop();

        //Debug.Log("Finish");


        //parentにRigidbody2Dを付けて、Ink消滅のカウントダウンtrueにする
        if (parent != null)
        {
            parent.GetComponent<Ink>().countDown = true;

            Rigidbody2D rb = parent.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                //重力が無い場合は、追加する
                parent.gameObject.AddComponent<Rigidbody2D>();

                //[ Inkの重さ×Inkオブジェクトの数 ]で重さを設定
                parent.GetComponent<Rigidbody2D>().mass = inkMass * inkQueue.Count;
            }
        }



        //Queueに保存されているInkのレイヤーを変更する
        GameObject ink;
        int count = inkQueue.Count;
        for (int i = 0; i < count; i++)
        {
            ink = inkQueue.Dequeue();
            ink.layer = layer;
        }


        //描くのを終了
        paint = false;

        //プレイヤーは動けるようになる
        playerController.playerMove = true ;
    }
}