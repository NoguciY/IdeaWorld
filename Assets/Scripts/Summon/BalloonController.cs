using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

/*
 生成されたら滞空する
 プレイヤーと当たると、プレイヤーはぶら下がり状態になり
 風船は浮遊する
 浮遊しているときは、ラインの重さによって上昇力が変わる
 他のオブジェクトに当たると退場する
 ぶら下がり状態のプレイヤーは左右の移動速度が下がり、ジャンプができなくなる
 プレイヤーが倒されたらぶら下がり状態を解除する
*/

public class BalloonController : SummonedObjController
{
    public float AxisH { get; set; }
    public float speedX = 0;        //横方向の速度

    [SerializeField] private float slightlyIncreasedBuoyancy = 1.4f;    //風船の浮力(小増加)
    [SerializeField] private float fairlyIncreasedBuoyancy = 2.1f;      //風船の浮力(中増加)
    [SerializeField] private float greatlyIncreasedBuoyancy = 3.5f;     //風船の浮力(大増加)

    float sppedY;                       //縦方向の速度
    float scaleX;                       //横スケール
    bool checkHitPlayer = false;        //プレイヤーと当たったか
    bool checkHitObj = false;           //オブジェクトに当たったか
    bool checkPlayer = false;           //プレイヤーに物が当たったか
    bool isHittingCollider = false;     //プレイヤーがオブジェクトと衝突したか
    Rigidbody2D rb;
    Rigidbody2D playerRb;
    BoxCollider2D boxCollide;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        sppedY = speed;
        rb = GetComponent<Rigidbody2D>();
        boxCollide = GetComponent<BoxCollider2D>();
        scaleX = transform.localScale.x;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //向きを変える
        if (AxisH > 0)
        {
            transform.localScale = new Vector2(scaleX, transform.localScale.y);
        }
        else if (AxisH < 0)
        {
            transform.localScale = new Vector2(-scaleX, transform.localScale.y);
        }

        //プレイヤーとオブジェクトとの当たり判定を調べる
        CheckPlayersCollider();

        //風船を破棄する
        DestroySummonedObj();
    }

    private void FixedUpdate()
    {
        if (checkHitPlayer)
        {
            //プレイヤーに当たった場合、スキル発動
            SummonedObjSkill();
        }
    }

    protected override void SummonedObjSkill()
    {
        if (rb.isKinematic)
        {
            //風船の物理演算を有効化
            rb.isKinematic = false;
        }

        //移動する
        rb.velocity = new Vector2(speedX, speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //線以外に当たった場合(即死オブジェクト、天井、地面、敵)
        if (collision.gameObject.CompareTag("DeadObject")   ||
            collision.gameObject.CompareTag("Ceiling")      ||
            collision.gameObject.CompareTag("Ground")       ||
            collision.gameObject.CompareTag("Enemy")        ||
            collision.gameObject.CompareTag("Enemies"))
        {
            checkHitObj = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //プレイヤーに当たった場合
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            playerController = collision.gameObject.GetComponent<PlayerController>();
            
            //プレイヤーの速度を0にする
            playerRb.velocity = Vector2.zero;

            //プレイヤーの重力をなくす  
            playerRb.gravityScale = 0;

            //プレイヤーが左向きの場合、風船を左向きにする
            if (collision.gameObject.transform.localScale.x < 0)
            {
                this.transform.localScale *= new Vector2(-1.0f, 1.0f);
            }

            //プレイヤーの親を風船(自分)にする
            collision.gameObject.transform.parent = gameObject.transform;

            //プレイヤーに当たったフラグを立てる
            checkHitPlayer = true;      
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //線が当たっている間、浮力を強くする
        if (collision.gameObject.CompareTag("LineParent"))
        {
            float lineMass = collision.gameObject.GetComponent<Rigidbody2D>().mass;
            //Debug.Log("Mass" + lineMass);

            if (lineMass > 1.0f && lineMass <= 3.0f) 
            {
                speed = slightlyIncreasedBuoyancy;
            }            
            else if (lineMass > 3.0f && lineMass <= 18.0f)
            {
                speed = fairlyIncreasedBuoyancy;
            }           
            
            else if (lineMass > 18.0f)
            {
                speed = greatlyIncreasedBuoyancy;
            }           
        }               
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //線が離れた瞬間、浮力を元に戻す
        if (collision.gameObject.CompareTag("LineParent"))
        { speed = sppedY; }
    }

    protected override void DestroySummonedObj()
    {
        //プレイヤーと線以外のオブジェクトに当たった場合 または
        //オプションのチェックリストに戻るボタンが押された場合
        if (checkHitObj || checkPlayer)
        {
            if (checkHitPlayer)
            {
                //プレイヤーの重力を元に戻す
                playerRb.gravityScale = 1;

                //親子関係解消
                gameObject.transform.DetachChildren();
            }

            //風船を破棄
            Destroy(gameObject);
        }

        //寿命が来た場合
        if (lifeTime > lifeSpan && !checkHitPlayer)
        {
            Destroy(gameObject);
        }
    }

    //プレイヤーとオブジェクトとの当たり判定を調べる
    void CheckPlayersCollider()
    {
        if (playerController != null)
        {
            isHittingCollider = playerController.IsHittingCollider;

            if (isHittingCollider)
            {
                checkPlayer = true;
            }
        }
    }
}
