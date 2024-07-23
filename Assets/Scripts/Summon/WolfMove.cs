using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//狼に関するクラス

public class WolfMove : MonoBehaviour
{
    public float keeppFromPlayerDistance = 2.0f;            //狼がプレイヤーと保つ距離
    public float wolfSpeed = 2.0f;                          //１フレームの移動量
    public float rayLength = 1.5f;                          //Rayの長さ
    public Vector2 maxJumpVec = new Vector2(2.0f, 6.0f);    //大ジャンプのベクトル
    public LayerMask objectLayer;                           //このレイヤーのオブジェクトのタグでジャンプするか見分ける

    GameObject player;          //プレイヤー
    WolfSkill wolfSkill;        //敵を見つけたら教えてもらうため
    Rigidbody2D rigidb;         //ジャンプする時使う
    Animator animator;
    float scaleX;               //横スケール
    float toPlayerDistance;     //プレイヤーとの距離
    Vector2 playerPos;          //プレイヤーの座標
    Vector2 wolfPos;            //狼(自身)の座標
    bool canJump = false;       //ジャンプできるか
    bool onGround = false;      //地上にいるか
    bool wolfDirection;         //trueが右方向
    bool jumping = false;       //ジャンプ中かどうか
    bool onLine = false;        //ライン上にいるか
    bool canWait;               //待機状態にできるか

    //狼の状態
    enum WolfState
    {
        Wait,     //待機
        Run,      //走る
        Jump      //ジャンプ
    }

    WolfState wolfState = WolfState.Wait;   //始めは待機状態

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //プレイヤーオブジェクトを取得
        player = GameObject.FindWithTag("Player");
        //敵を監視するためのスクリプト
        wolfSkill = GetComponent<WolfSkill>();
        //プレイヤーが振り向くときに使う
        scaleX = transform.localScale.x;

        rigidb = GetComponent<Rigidbody2D>();

        //狼の向きを決める
        TurnWolfDirection();

        //待機状態にする
        ChangeState(WolfState.Wait);
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーがいて、敵がいない場合
        if (player && !wolfSkill.enemy)
        {
            //プレイヤーと狼の座標を取得
            playerPos = player.transform.position;
            wolfPos = transform.position;

            //狼とプレイヤーとの距離
            toPlayerDistance = playerPos.x - wolfPos.x;

            //狼の向きを決める
            TurnWolfDirection();

            //地上判定
            CheckOnGround();

            //着地点の地上判定
            CheckCanLandInGround();

            //状態を更新する
            StateUpdate();

            //状態によって狼を更新する
            switch (wolfState)
            {
                case WolfState.Wait:

                    if (animator.GetBool("isRunning"))
                    {
                        //走りアニメ停止
                        animator.SetBool("isRunning", false);
                    }
                    //Debug.Log("WAIT");
                    break;

                case WolfState.Run:
                    //プレイヤーと距離を保つように動く
                    KeepDistance();

                    if (!animator.GetBool("isRunning"))
                    {
                        //走りアニメ再生
                        animator.SetBool("isRunning", true);
                    }
                    //Debug.Log("RUN");
                    break;

                case WolfState.Jump:
                    if (animator.GetBool("isRunning"))
                    {
                        //走りアニメ停止
                        animator.SetBool("isRunning", false);
                    }
                    //ジャンプする
                    Jump();

                    //Debug.Log("JUMP");
                    break;

                default:
                    break;
            }
        }
    }

    //状態を変える関数
    void ChangeState(WolfState nextState) => wolfState = nextState;

    //狼の進行方向を調べる関数
    //true  :右方向(正)に進む
    //false :左方向(負)に進む
    void TurnWolfDirection()
    {
        //プレイヤーが右を向いている場合
        if (player.transform.localScale.x > 0)
        {
            //右を向く
            wolfDirection = true;
            transform.localScale = new Vector2(scaleX * 1, transform.localScale.y);
        }
        //プレイヤーが左を向いている場合
        else
        {
            //左を向く
            wolfDirection = false;
            transform.localScale = new Vector2(scaleX * -1, transform.localScale.y);
        }
    }

    //地上にいるかの判定
    void CheckOnGround()
    {
        Ray2D wolfFrontRay;         //狼の前に設ける光線
        Vector2 startRayPos;        //光線の始点
        if (wolfDirection)
        {
            //光線の始点 = 狼の鼻先辺り (右向き)
            startRayPos = transform.position + new Vector3(0.7f, 0);
        }
        else
        {
            //光線の始点 = 狼の鼻先辺り (左向き)
            startRayPos = transform.position + new Vector3(0.7f, 0) * -1.0f;
        }

        //光線を取得
        wolfFrontRay = new Ray2D(startRayPos, Vector2.down);

        //Rayに当たったオブジェクトレイヤーの情報を格納
        //Physics2D.Raycast(始点, 方向, rayの長さ, 対象レイヤー)関数
        //返り値:RaycastHit2D
        RaycastHit2D hit = Physics2D.Raycast(wolfFrontRay.origin,
                  wolfFrontRay.direction, rayLength, objectLayer);

        if (hit.collider)
        {
            //Rayに地面が当たっている場合
            if (hit.collider.tag == "Ground")
            {
                //地上
                onGround = true;
                onLine = false;
            }
            else if (hit.collider.tag == "Line")
            {
                onLine = true;
                onGround = false;
            }
            //Rayに地面が当たっていない場合
            else { onGround = false; onLine = false; }
        }
        else { onGround = false; onLine = false; }
    }

    //ジャンプして着地できるかを調べる関数
    void CheckCanLandInGround()
    {
        Ray2D fromLandingPointRay;  //着地点を調べる光線
        Vector2 landingPoint;       //ジャンプの着地点
        //ジャンプの飛距離
        float jumpingDistance = maxJumpVec.x * (maxJumpVec.y / (-Physics2D.gravity.y * 0.5f));

        if (wolfDirection)
        {
            //光線の始点 = ジャンプの着地点(右向き)
            landingPoint = (Vector2)transform.position + new Vector2(jumpingDistance + 0.7f, 0);
        }
        else
        {
            //光線の始点 = ジャンプの着地点(左向き)
            landingPoint = (Vector2)transform.position + new Vector2(jumpingDistance + 0.7f, 0) * -1.0f;
        }

        //ジャンプの着地点を始点とする光線を取得
        fromLandingPointRay = new Ray2D(landingPoint, Vector2.down);

        //Rayに当たったオブジェクトレイヤーのオブジェクト情報を格納
        RaycastHit2D groundHit = Physics2D.Raycast(fromLandingPointRay.origin,
                     fromLandingPointRay.direction, rayLength, objectLayer);

        if (groundHit.collider)
        {
            //Rayが地面に当たった場合、ジャンプ可能
            if (groundHit.collider.tag == "Ground")
            {
                canJump = true;
            }
            //Rayがラインに当たった場合、ジャンプ可能
            else if (groundHit.collider.tag == "Line")
            {
                canJump = true;
            }
            //ジャンプ不可
            else { canJump = false; }
        }
        else { canJump = false; }
    }

    //走る状態で実行する関数
    //敵を見つけるまではプレイヤーと距離を保ちつつ移動
    void KeepDistance()
    {
        Vector2 moveVec = transform.position;

        //狼が右向きの場合
        if (wolfDirection)
        {
            //右に移動
            moveVec.x += wolfSpeed * Time.deltaTime;
        }
        //狼が左向きの場合
        else
        {
            moveVec.x -= wolfSpeed * Time.deltaTime;
        }
        transform.position = moveVec;
    }

    //ジャンプ状態で実行する関数
    //ジャンプしている間は、プレイヤーの影響で動かない
    void Jump()
    {
        //ジャンプしていない
        if (!jumping)
        {
            if (wolfDirection)
            {
                //右に大ジャンプ
                rigidb.AddForce(maxJumpVec, ForceMode2D.Impulse);
            }
            else
            {
                //左に大ジャンプ
                rigidb.AddForce(maxJumpVec * new Vector2(-1, 1), ForceMode2D.Impulse);
            }

            //ジャンプアニメ再生
            animator.SetTrigger("JumpTrigger");
            //ジャンプ中
            animator.SetBool("isJumping", true);
            Debug.Log("Jump");
            //ジャンプ状態でジャンプ中じゃなく、ジャンプした後
            jumping = true;
        }
    }

    //状態を走る状態に変える関数
    bool ChangeRunState()
    {
        //Rayが地面か線に当たっている場合
        if (onGround || onLine)
        {
            //右向きの場合
            if (wolfDirection)
            {
                //プレイヤーと狼が保つ座標より狼の座標が小さい場合
                if (playerPos.x + keeppFromPlayerDistance > wolfPos.x)
                {
                    return true;
                }
            }
            //左向きの場合
            else
            {
                //プレイヤーと狼の距離が-2mより大きい場合
                if (playerPos.x - keeppFromPlayerDistance < wolfPos.x)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //状態を待機状態に変える関数
    bool ChangeWaitState()
    {
        //Rayが地面に当たっていない場合 かつ
        //目の前に飛び越えられない穴か壁がある場合
        if (!onGround && !canJump)
        { return true; }
        //狼がプレイヤーの2m前にいる場合
        else { return true; }
    }

    //状態をジャンプ状態に変える関数
    bool ChangeJumpState()
    {
        if (!jumping)
        {
            //地上についてないかつジャンプ可能
            if (!onGround && canJump)
            {
                return true;
            }
        }
        return false;
    }

    //状態を更新する関数
    void StateUpdate()
    {
        if (ChangeJumpState())
        {
            //ジャンプ状態に移行
            ChangeState(WolfState.Jump);
        }
        else if (ChangeRunState())
        {
            //走る状態に移行
            ChangeState(WolfState.Run);
        }
        else if (ChangeWaitState())
        {
            //待機状態に移行
            ChangeState(WolfState.Wait);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //地面に当たった瞬間
        if (collision.gameObject.tag == "Ground")
        {
            //ジャンプしていない
            jumping = false;
        }

        //ジャンプアニメから着地アニメへ遷移
        if (animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", false);
        }
    }
}