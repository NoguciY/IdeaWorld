using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField,Tooltip("現在の体力")] int life;        //体力
    [SerializeField,Tooltip("体力の設定")] int maxLife;     //最大体力
    [HideInInspector]public Vector2 checkPoint;             //チェックポイントの場所を記録
    [HideInInspector]public bool invincible=false;

    [Header("参照系")]
    [SerializeField]GameManager gameManager;
    [SerializeField]MarkerManager markerManager;
    [SerializeField]PlayerController playerController;

    public GameObject optionButton;

    private void Start()
    {
        //体力を設定
        life = maxLife;

        //チェックポイントを設定
        checkPoint = transform.position;
    }

    public void PlayerDamage(int damage)
    {
        //体力を減らす
        life -= damage;

        //死んだ場合、体力を回復させて最後に通過したチェックポイントで復活
        if (life <= 0)
        {
            //描いてる途中の線を描くのを終了する
            markerManager.PaintFinish();

            //ステージ内のすべての線を消す
            markerManager.RemoveAllLines();

            gameManager.SwitchingMarkersAndSketches(true);

            //体力回復
            life = maxLife;

            //ひとつ前のチェックポイントに戻す
            transform.position = checkPoint;

            //オプションボタンも表示する(スケッチモード中死んだときのため)
            optionButton.SetActive(true);
        }
    }

    //現在のライフを取得する関数
    public int GetLife()
    {
        return life;
    }

    //とげに当たると即死
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //棘に当たった場合
        if (collision.gameObject.tag == "DeadObject")
        {
            PlayerDamage(3);
        }
        //岩に当たった場合
        else if (collision.gameObject.tag == "Rock")
        {
            PlayerDamage(1);
        }
    }

    //チェックポイントに戻る
    public void ReturnCheckpoint()
    {
        //プレイヤーがぶら下がり状態の場合
        if (playerController.IsHanging)
        {
            //風船を破壊する
            playerController.OnDestroyBalloonFlag(checkPoint);
        }

        markerManager.PaintFinish();
        gameManager.SwitchingMarkersAndSketches(true);
        life = maxLife;
        transform.position = checkPoint;

        optionButton.SetActive(true);
        optionButton.GetComponent<OptionButtons>().OpsionButtonsPanelSwitching(false);

        Time.timeScale = 1;
    }
}