using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//狼(召喚物)に関するクラス

//プレイヤーの2m前に生成される
//スキル：敵の動きを止める(３秒間)
//敵が狼の半径5m以内に入ったら、敵に近づき
//敵が半径1m以内に入ったら、スキル発動
//スキル発動後、または召喚されて10秒間スキルを発動しなかった場合に消える
//↓
//敵の攻撃に5回当たった場合消滅する
//スキル発動後、プレイヤーの前に戻る
//戻ってきてから10秒後スキルは発動できない

//スキル発動時
//吠えるモーションの70フレーム目で3秒間待機する

public class WolfSkill : SummonedObjController
{
    public GameObject enemy;                        //敵のオブジェクト
    public LayerMask enemyLayer;                    //検知したいレイヤー
    public float radius = 5.0f;                     //敵を検知する半径
    public float activeSkillDistance = 4.0f;        //スキルが発動する狼(自分)と敵の距離
    public float animationPauseTime = 0.7f;         //狼のアニメーションを一時停止する時間
    public float barkAnimationRestartTime = 1.5f;   //咆哮アニメーションの再開時間
    public float finishAnimeTime = 3.0f;            //敵複数時の咆哮アニメーションが終わる時間

    const float skillDurationTime = 4.4f;           //スキルの継続時間
    const float wolfRadius = 1.0f;                  //狼の半径
    const int health = 5;                           //狼の体力

    bool activedSkill = false;                      //スキルが発動したか
    bool endSkill = false;                          //スキルが終わったか
    bool canSkill = true;                           //スキルを発動できるか
    bool canRestartAnimation = false;               //アニメを再開できるか
    bool restartAnimation = false;                  //アニメを再開しているか
    int damageCount = 0;                            //被ダメージ数
    float distanceToEnemy;                          //狼(自分)と敵の距離
    float toEnemyDir;                               //敵への方向を知るため
    float skillCoolTime = 10.0f;                    //スキル発動のための待機時間
    float afterSkillDeltaTime = 0.0f;               //スキル発動後の経過時間
    string animationStateName = "Bark";             //一時停止させるアニメーション名

    Animator animator;
    EnemyController enemyController;                //敵(単体)のコンポーネント
    EnemiesController enemiesController;            //敵(複数)のコンポーネント
    RaycastHit2D hitObjectInfo;                     //Rayに当たったオブジェクトの情報
    WolfEffectController wolfEffectController;      //エフェクトを表示させるスクリプト
    AudioSource barkingAudioSource;                 //スキル発動時に使う
    
    void Start()
    {
        //敵が見つかるまではnullオブジェクト
        enemy = null;
        animator = GetComponent<Animator>();
        barkingAudioSource = GetComponent<AudioSource>();
        wolfEffectController = transform.Find("WolfEffect").gameObject.GetComponent<WolfEffectController>();
    }

    protected override void Update()
    {
        base.Update();
        
        //スキル発動可能な場合
        if (canSkill)
        {
            //敵を検知する
            DetectEnemy();
        }
        //敵が検知できないかつスキル終了後
        else if (!canSkill && endSkill)
        {
            afterSkillDeltaTime += Time.deltaTime;

            if (afterSkillDeltaTime >= skillCoolTime)
            {
                afterSkillDeltaTime = 0.0f;
                lifeTime = 0.0f;

                //クールタイム後、敵の検知とスキル発動をできるようにする
                activedSkill = false;
                canSkill = true;
                endSkill = false;
                //Debug.Log("スキル発動可能");
            }
        }

        //アニメーションの再生時間をカウントして任意のタイミングになった場合知らせる
        //引数：指定するアニメーション、指定するタイミング
        canRestartAnimation = CountAnimationTime(animationStateName, animationPauseTime);

        //咆哮停止タイミングが来た場合
        if (canRestartAnimation && !restartAnimation)
        {
            restartAnimation = true;

            //咆哮アニメの一時停止
            TakePauseAnimation();

            //咆哮アニメの再開
            Invoke("RestartBarkAnimation", barkAnimationRestartTime);
        }

        //敵を見つけた場合、スキル発動
        SummonedObjSkill();

        //破棄する
        DestroySummonedObj();
    }

    protected override void SummonedObjSkill()
    {
        //敵を見つけた場合
        if (enemy)
        {
            //狼と敵の距離の測定
            distanceToEnemy = (transform.position - enemy.transform.position).magnitude - wolfRadius;

            //狼から敵のｘ方向の距離を計算
            toEnemyDir = enemy.transform.position.x - transform.position.x;

            if (!activedSkill)
            {
                //敵との距離がスキル発動範囲内の場合
                if (distanceToEnemy <= activeSkillDistance)
                {
                    activedSkill = true;

                    //敵単体の場合と複数の場合で処理を変更
                    if (hitObjectInfo.collider.gameObject.CompareTag("Enemy"))
                    {
                        //敵単体の場合
                        //敵の移動を司るコンポーネントを取得
                        enemyController = enemy.GetComponent<EnemyController>();
                        //猿のエフェクトを変化させる
                        enemyController.activeWolfSkill = true;

                        //狼の吠えるアニメ再生
                        animator.SetTrigger("BarkTrigger");

                        //狼のエフェクトを表示する
                        wolfEffectController.ActiveControllImageAFlag();
                        
                        //吠えるSE再生
                        barkingAudioSource.Play();

                        //狼と猿のエフェクトを消し、敵を動かす
                        Invoke("StopOff", skillDurationTime);
                        
                    }
                    else if (hitObjectInfo.collider.gameObject.CompareTag("Enemies"))
                    {
                        //敵複数の場合
                        enemiesController = enemy.GetComponent<EnemiesController>();

                        //狼の吠えるアニメ再生
                        animator.SetTrigger("BarkTrigger");

                        //エフェクトを表示する
                        wolfEffectController.ActiveControllImageAFlag();

                        //吠えるSE再生
                        barkingAudioSource.Play();

                        //猿のエフェクトを変化させる
                        enemiesController.ActiveBarkingFlag();

                        //吠えるアニメの後に敵が退散する
                        Invoke("RunAwayEnemies", finishAnimeTime);
                    }
                }
                //敵との距離がスキル発動範囲外の場合
                else if (distanceToEnemy > activeSkillDistance)
                {
                    //Debug.Log("狼と敵の距離:" + distanceToEnemy);
                    //敵の方へ移動する
                    GoToEnemy();
                }
            }
        }
    }

    //敵を検知する関数
    void DetectEnemy()
    {
        Vector2 startPos = (Vector2)transform.position + (Vector2.up * 0.6f);   //開始点
        Vector2 direction = Vector2.zero;                                       //投射する方向
        float maxDistance = 0.0f;                                               //投射する最大距離

        hitObjectInfo = Physics2D.CircleCast(startPos, radius, direction, maxDistance, enemyLayer);

        if (hitObjectInfo)
        {
            //敵のオブジェクト取得
            enemy = hitObjectInfo.collider.gameObject;
            //Debug.Log("敵を検知");
            canSkill = false;           //敵の検知をできなくする
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere
            (transform.position + (Vector3.up * 0.6f),radius);
    }

    //敵の方へ移動する関数
    void GoToEnemy()
    {
        //敵が右にいる場合
        if (toEnemyDir > 0)
        {
            //右に移動
            transform.position += transform.right * speed * Time.deltaTime;
        }
        //敵が左にいる場合
        else if (toEnemyDir < 0)
        {
            //左に移動
            transform.position -= transform.right * speed * Time.deltaTime;
        }
    }

    //狼の吠えるアニメ待ちをする
    void RunAwayEnemies()
    {
        enemiesController.ActiveRunAwayEnemies();
        enemy = null;
        endSkill = true;
    }

    //敵を再度動かす関数
    void StopOff()
    {
        //敵の表情とエフェクトを変える
        enemyController.activeWolfSkill = false;

        //スキル終了フラグオン
        endSkill = true;

        //敵をリセット
        enemy = null;
        restartAnimation = false;
        Debug.Log("敵、再活動");
    }

    //アニメーションの再生時間をカウントして指定したタイミングを知らせる関数
    bool CountAnimationTime(string animationStateName, float specifiedTiming)
    {
        float animationTime;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
        {
            //吠えるアニメーションの情報を取得
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] animatorClip = animator.GetCurrentAnimatorClipInfo(0);
            //吠えるアニメの現在の再生時間を取得
            animationTime = animatorClip[0].clip.length * animationState.normalizedTime;
        }
        else
        {
            animationTime = 0;
        }

        //アニメーションの経過時間が指定した時間以上の場合
        if (animationTime >= specifiedTiming)
        { return true; }
        else { return false; }
    }

    //アニメーションを一時停止する関数
    void TakePauseAnimation()
    {
        //アニメーションの一時停止
        animator.SetFloat("BarkingSpeed", 0.0f);
    }

    //アニメーションを再始動する関数
    void RestartBarkAnimation()
    {
        animator.SetFloat("BarkingSpeed", 1.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //敵に当たった場合、被ダメ＋１
        if (collision.gameObject.CompareTag("Enemy"))
        { damageCount++; }
    }

    //召喚物を破棄する関数
    protected override void DestroySummonedObj()
    {
        //敵の攻撃に5回当たった場合、破棄
        if (damageCount >= health)
        {
            Destroy(gameObject);
        }
    }
}