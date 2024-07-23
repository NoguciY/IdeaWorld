using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;              //状態遷移の管理をするクラス

    [SerializeField] private LayerMask objectLayer;     //着地できるレイヤー

    [Header("移動速度")]
    [SerializeField] float moveSpeed;                   //移動速度

    [Header("最大移動速度")]
    [SerializeField] float maxMoveSpeed;                //速度を制限する

    [Header("ジャンプ力")]
    [SerializeField] float jumpForce;                   //ジャンプ力

    [Header("最大ジャンプ力")]
    [SerializeField] float maxJumpForce;                //ジャンプ力を制限する

    [SerializeField] AudioSource runningAudioSource;    //足音
    
    [SerializeField] AudioSource jumpingAudioSource;    //ジャンプ音

    [SerializeField] PlayerManager playerManager;       //プレイヤー管理用

    Rigidbody2D rigidb;                                 //物理演算用
    bool onGround = false;                              //地面の上にいるか
    float originalScaleX;                               //プレイヤーの元々の横スケール
    float scaleX;                                       //横スケール
    float axisH;                                        //水平方向の入力
    float runSpeed;                                     //走る速度
    float jumpSpeed;                                    //ジャンプの速度
    bool jumpKey;                                       //ジャンプキーが押されたか
    bool isPressedHintKey;                              //ヒントキーが押されたか
    bool isHittingCollider;                             //コライダーに当たっているか
    public bool playerMove = true;                      //プレイヤーが操作可能か
    public bool knockback = false;                      //ノックバックするか
    BalloonController balloonController = null;         //風船アクセス用
    CapsuleCollider2D capsuleCollider;                  //当たり判定
    PhysicsMaterial2D originalMaterial;                 //摩擦力を変更するため

    //アクセサ
    public bool OnGround => onGround;
    public bool JumpKey => jumpKey;
    public bool IsPressedHintKey => isPressedHintKey;
    public bool IsHittingCollider => isHittingCollider;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float AxisH => axisH;
    public float OriginalScaleX => originalScaleX;
    public BalloonController BalloonController => balloonController;
    public Rigidbody2D Rigidb => rigidb;
    public AudioSource RunningAudioSource => runningAudioSource;
    public AudioSource JumpingAudioSource => jumpingAudioSource;

    public float RunSpeed
    {
        get { return runSpeed; }
        set { runSpeed = value; }
    }

    //ジャンプ中か
    public bool IsJumping { get; set; } = false;
    //風船に当たったか
    public bool HitBalloon { get; set; } = false;
    //ぶら下がり中か
    public bool IsHanging { get; set; } = false;

    public Animator Animator { get; private set; }

    private void Awake()
    {
        playerStateMachine = new PlayerStateMachine();
        playerStateMachine.Init(this, State.Idle);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        scaleX = transform.localScale.x;

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalMaterial = capsuleCollider.sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        //接地判定
        CheckOnGround();

        //ぶら下がり状態でないときはisHittingColliderはfalseにする
        if (!IsHanging && isHittingCollider)
        {
            //物が当たっていない
            isHittingCollider = false;
        }

        //描いてるときは動けない
        if (playerMove)
        {
            //ジャンプキーの入力を検知する
            jumpKey = Input.GetButtonDown("Jump");

            //a,d,←,→キーの入力を検知する
            axisH = Input.GetAxis("Horizontal");

            //ヒント表示ボタンの入力を検知する
            isPressedHintKey = Input.GetKeyDown(KeyCode.S);

            //ぶら下がり中以外はプレイヤーが振り向くようにする
            if (!balloonController)
            {
                if (axisH > 0)
                {
                    //右を向く
                    transform.localScale = new Vector2(scaleX, transform.localScale.y);
                }
                else if (axisH < 0)
                {
                    //左を向く
                    transform.localScale = new Vector2(-scaleX, transform.localScale.y);
                }
            }
        }

        //現在の状態を更新する
        playerStateMachine.Update();
    }

    private void FixedUpdate()
    {

        if (playerMove)
        {
            //ノックバックの処理
            if (knockback)
            {
                StartCoroutine(KnockBack());
                return;
            }
            //ノックバックしていない場合
            else
            {
                //ダッシュとジャンプをする
                rigidb.velocity += new Vector2(runSpeed, jumpSpeed);

                //移動速度を制限する
                if (rigidb.velocity.x > maxMoveSpeed)
                {
                    rigidb.velocity = new Vector2(maxMoveSpeed, rigidb.velocity.y);
                }
                else if (rigidb.velocity.x < -maxMoveSpeed)
                {
                    rigidb.velocity = new Vector2(-maxMoveSpeed, rigidb.velocity.y);
                }

                //ジャンプ力を制限する
                if (rigidb.velocity.y > maxJumpForce)
                {
                    rigidb.velocity = new Vector2(rigidb.velocity.x, maxJumpForce);
                }
            }
        }
        //線を描いているときの処理
        else
        {
            //横移動できなくする
            rigidb.velocity = Vector2.up * rigidb.velocity;
        }

        //地上時は摩擦力をありにして
        //そのほかでは摩擦力をなくす
        if (onGround)
        {
            // 一時的なコピーを作成して変更
            PhysicsMaterial2D temporaryMaterial = new PhysicsMaterial2D();
            temporaryMaterial.friction = 0.4f;

            // 一時的なコピーをColliderに設定
            capsuleCollider.sharedMaterial = temporaryMaterial;
        }
        else
        {
            // オリジナルの物理マテリアルに戻す
            capsuleCollider.sharedMaterial = originalMaterial;
        }
    }

    //地上判定をする関数
    void CheckOnGround()
    {
        //プレイヤーの原点からのY軸の距離
        float underCheckOffsetY = 0.2f;

        //円状Rayの原点
        Vector2 underCheckOrigin = (Vector2)transform.position + (Vector2.up * underCheckOffsetY);

        //円状Rayの半径
        float underCheckRadius = 0.22f;

        //円状Rayの距離
        float underCheckDistance = 0.0f;

        //Rayに当たったオブジェクト情報を格納する
        RaycastHit2D underHit = Physics2D.CircleCast(underCheckOrigin, underCheckRadius,
                                                     Vector2.down, underCheckDistance, objectLayer);

        //Rayに指定したレイヤーが当たった場合
        if (underHit.collider)
        {
            //地面が当たった場合
            if (underHit.collider.gameObject.tag == "Ground")
            {
                //地上にいる
                onGround = true;
            }
            //ラインが当たった場合
            if (underHit.collider.gameObject.tag == "Line")
            {
                //地上にいる
                onGround = true;
            }
        }
        //地上にいない
        else { onGround = false; }
    }

    //カプセルレイデバッグ用
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(
            transform.position + (Vector3.up * 0.2f),
            0.22f);
    }

    IEnumerator KnockBack()
    {
        //0.5秒後、ノックバックフラグオフ
        yield return new WaitForSeconds(0.5f);
        knockback = false;
    }

    public bool OnDestroyBalloonFlag(Vector2 checkPoint)
    {
        //風船ごとチェックポイントに戻す
        balloonController.transform.position = checkPoint;

        //風船を壊すためのフラグオン
        isHittingCollider = true;
        return isHittingCollider;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsHanging)
        {
            //地面、天井、線、敵、即死オブジェクトに当たった場合
            if (collision.gameObject.CompareTag("Ground")       ||
                collision.gameObject.CompareTag("Ceiling")      ||
                collision.gameObject.CompareTag("LineParent")   ||
                collision.gameObject.CompareTag("Enemy")        ||
                collision.gameObject.CompareTag("Enemies")      ||
                collision.gameObject.CompareTag("DeadObject"))
            {
                //風船を壊すためのフラグオン
                isHittingCollider = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //風船に当たった場合
        if (collision.gameObject.CompareTag("Balloon"))
        {
            balloonController = collision.gameObject.GetComponent<BalloonController>();
            HitBalloon = true;
        }

        if (collision.gameObject.CompareTag("DeadObject"))
        {
            //風船を壊すためのフラグオン
            isHittingCollider = true;

            //プレイヤーのライフを0にする
            playerManager.PlayerDamage(3);
        }
    }

    //各状態に移行させる関数
    public void Idle() => playerStateMachine.ChangeState(State.Idle);
    public void Run() => playerStateMachine.ChangeState(State.Run);
    public void Jump() => playerStateMachine.ChangeState(State.Jump);
    public void Hang() => playerStateMachine.ChangeState(State.Hang);
}
