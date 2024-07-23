using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//猿(複数)の動きに関するクラス

//プレイヤーまたは狼が近づくと表情が変わる
//離れると通常時に戻る

//狼の吠える攻撃で動きを止めて、
//怯えエフェクトが出る
//左側に移動
//その後、数秒後に消える

public class EnemiesController : MonoBehaviour
{
    [SerializeField] float runSpeed;                    //逃げる速度
    [SerializeField] float extinctionTime;              //消滅時間
    [SerializeField] GameObject[] effectSprites;        //敵のエフェクト
    [SerializeField] private GameObject targetObject;   //ターゲットのゲームオブジェクト
    [SerializeField] private float searchRange;         //敵の索敵範囲
    [SerializeField] float untilBarkWaitingTime = 0.8f; //吠えるまでの待ち時間

    const int maxColorA = 255;                          //画像の最大α値

    bool isBarkingFromWolf = false;                     //狼から吠えられたか
    bool activerRunAwayEnemies = false;                 //敵を退散させるか
    bool targetFinding = false;                         //ターゲットを見つけたか
    float deltaTime;                                    //経過時間

    public bool TargetFinding
    {
        get { return targetFinding; }
        private set { targetFinding = value; }
    }

    public bool IsBarkingFromWolf
    {
        get { return isBarkingFromWolf; }
        private set { isBarkingFromWolf = value; }
    }

    private void Update()
    {
        //ターゲットが索敵範囲内の場合
        if (Mathf.Abs(transform.position.x - targetObject.transform.position.x) <= searchRange)
        {
            if (!targetFinding)
            { TargetFinding = true; }
        }

        //狼がスキル発動した場合
        if (isBarkingFromWolf)
        {
            isBarkingFromWolf = false;
            //エフェクトを表示する
            StartCoroutine(DisplayEffect());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activerRunAwayEnemies)
        {
            RunAwayEnemies();
        }
    }
    
    //狼から吠えられた場合フラグオン
    public void ActiveBarkingFlag()
    {
        isBarkingFromWolf = true;
    }

    //狼から吠えられた場合フラグオン
    public void ActiveRunAwayEnemies()
    {
        activerRunAwayEnemies = true;
    }

    //猿を退散させる
    void RunAwayEnemies()
    {
        deltaTime += Time.deltaTime;
        transform.position += Vector3.right * runSpeed;

        if (deltaTime > extinctionTime)
        {
            Destroy(gameObject);
        }
    }

    //エフェクトの画像を表示する
    IEnumerator DisplayEffect()
    {
        yield return new WaitForSeconds(untilBarkWaitingTime);

        for (int i = 0; i < effectSprites.Length; i++)
        {
            effectSprites[i].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, maxColorA);
        }
    }
}
