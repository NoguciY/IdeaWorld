using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public bool activeWolfSkill = false;    //狼がスキル発動中か

    [Tooltip("枝の参照")]
    public GameObject branch;

    [Tooltip("投的の攻撃間隔")]
    public float interval;

    [Tooltip("エネミーの移動速度")]
    public float speed;

    [SerializeField, Tooltip("攻撃範囲(半径)")]
    private float range;

    [SerializeField, Tooltip("索敵範囲(半径)")] 
    float targetSearchRange;

    [SerializeField, Tooltip("発射位置をここに割り当てる")]
    Transform firingPosition;

    [SerializeField, Tooltip("行動範囲の微調整用")] 
    float offsetX = 1.0f;

    [SerializeField, Tooltip("アニメーションを一時停止する時間")] 
    float animationPauseTime = 0.7f;

    /// <summary>
    /// 射出するオブジェクト
    /// </summary>
    [SerializeField, Tooltip("射出するオブジェクトをここに割り当てる")]
    GameObject[] ThrowingObject;

    /// <summary>
    /// 標的のオブジェクト
    /// </summary>
    [SerializeField, Tooltip("標的のオブジェクトをここに割り当てる")]
    GameObject TargetObject;

    /// <summary>
    /// 射出角度
    /// </summary>
    [SerializeField, Range(0F, 90F), Tooltip("射出する角度")]
    float ThrowingAngle;

    int index = 0;                          //ThrowingObjectのindex
    float timeCount;                        //時間計測
    float time;                 
    float randamSpan;           
    float branchLeftEdgePosX;               //枝の左端の位置
    float branchRightEdgePosX;              //枝の右端の位置
    float branchPosx;                       //枝の中心座標
    float branchScaleX;                     //枝の横幅
    Vector3 scale;                          //プレイヤーの方を向くときに、スケール反転で使う
    Animator animator;                      //アニメーション用
    bool canRestartAnimation;               //アニメーションを再開できるか
    string animationStateName = "Attack";   //一時停止させるアニメーション名

    void Start()
    {
        //攻撃のインターバル
        timeCount = interval;

        //スケールを記憶
        scale = transform.localScale;

        //猿の移動制限のために枝の横幅と中心座標を取得
        if (branch)
        {
            branchScaleX = branch.GetComponent<BoxCollider2D>().size.x;
        }
        branchPosx = branch.transform.position.x;

        branchLeftEdgePosX = branchPosx - (branchScaleX / 2) + offsetX;
        branchRightEdgePosX = branchPosx + (branchScaleX / 2) - offsetX;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeWolfSkill)
        {
            //ターゲットが索敵範囲内の場合
            if (Mathf.Abs(transform.position.x - TargetObject.transform.position.x) <= targetSearchRange)
            {
                //右に移動
                if (transform.position.x + range < TargetObject.transform.position.x && branchRightEdgePosX > transform.position.x)
                {
                    transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
                }
                //左に移動
                else if (transform.position.x - range > TargetObject.transform.position.x && branchLeftEdgePosX < transform.position.x)
                {
                    transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
                }
            }

            if (Mathf.Abs(transform.position.x - TargetObject.transform.position.x) <= range)
            {
                timeCount -= Time.deltaTime;

                if (timeCount < 0)
                {
                    index = Random.Range(0, ThrowingObject.Length);
                    ThrowingBall(index);
                    timeCount = interval;
                }
            }

            //ターゲットの方を見る
            LookTarget();
        }

        //攻撃アニメーションの開始時間を判断する
        canRestartAnimation = IsIndicateSpecifiedAnimationTime(animationStateName, animationPauseTime);
    }

    private void ThrowingBall(int index)
    {
        if (ThrowingObject != null && TargetObject != null)
        {
            //攻撃を行う
            StartCoroutine(AttackCoroutine());
        }
        else
        {
            throw new System.Exception("射出するオブジェクトまたは標的のオブジェクトが未設定です。");
        }
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    //プレイヤーにダメージを与える
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();

            if (player != null)
            {
                player.PlayerDamage(1);
            }
        }
    }

    void LookTarget()
    {
        if (transform.position.x < TargetObject.transform.position.x)
        {
            transform.localScale = new Vector3(-scale.x, scale.y, 0);
        }
        else
        {
            transform.localScale = new Vector3(scale.x, scale.y, 0);
        }
    }

    //攻撃アニメーションの後数秒間待機させて攻撃をする
    IEnumerator AttackCoroutine()
    {
        //アニメーションを再生
        animator.SetTrigger("AttackTrigger");

        yield return new WaitForSeconds(1.2f);

        // Ballオブジェクトの生成
        GameObject ball = Instantiate(ThrowingObject[index], firingPosition.transform.position, Quaternion.identity);

        Vector3 targetPosition;

        if (Random.Range(0, 2) == 0)
        {
            // 標的の座標
            targetPosition = TargetObject.transform.position;
        }
        else
        {
            if (TargetObject.transform.localScale.x > 0)
            {
                targetPosition = TargetObject.transform.position + new Vector3(5, 0, 0);
            }
            else
            {
                targetPosition = TargetObject.transform.position - new Vector3(5, 0, 0);
            }
        }

        // 射出角度
        float angle = ThrowingAngle;

        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(firingPosition.transform.position, targetPosition, angle);

        // 射出
        Rigidbody2D rid = ball.GetComponent<Rigidbody2D>();
        rid.AddForce(velocity * rid.mass, ForceMode2D.Impulse);
    }

    //アニメーションの再生時間をカウントして指定したタイミングを知らせる関数
    bool IsIndicateSpecifiedAnimationTime(string animationStateName, float specifiedTiming)
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
}

