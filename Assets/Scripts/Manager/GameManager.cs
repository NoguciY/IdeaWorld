using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("参照関係")]
    [SerializeField] UIManager uIManager;

    [SerializeField] MarkerManager markerManager;

    [SerializeField] SketchManager sketchManager;

    [SerializeField] PlayerController playerController;

    [SerializeField] PlayerManager playerManager;



    [Header("OptionButtonの参照"),SerializeField] GameObject optionButton;
    [Header("OptionButtonsPanel参照"), SerializeField] GameObject OptionButtonsPanel;
    [Header("TutorialImage参照"), SerializeField] GameObject TutorialImage;



    [Header("Penの参照"), SerializeField]
    GameObject Pen;

    //フラワーキーの残りの数
    [HideInInspector]public int keysRemaining;


    //マーカーとスケッチの切り替えで使う
    bool sketchMode = false;

    //スケッチアニメーション中か
    bool isSketchAnimated = false;

    //再召喚までのインターバル用
    [HideInInspector]
    public float interval;

    private void Start()
    {
        //操作説明画面表示してあるから、taimScaleを0にする
        Time.timeScale = 0;

        //マウスの矢印カーソルを消す
        Cursor.visible = false;
        //最初はスケッチモードはOFF
        sketchManager.enabled = false;
        //再召喚までのインターバルの秒数を設定
        interval = sketchManager.sketchInterval;
    }
    void Update()
    {

        //マウスカーソルの位置に筆を移動
        Pen.transform.position = Input.mousePosition;
        //マウスカーソルの位置に線を引くためのposition調整
        markerManager.transform.position = MouseWorldPosition();


        if (Time.timeScale == 1)
        {

            //再召喚までのインターバル
            if (sketchManager.summons == false)
            {

                interval -= Time.deltaTime;

                //インターバルのTextを表示
                uIManager.DisplayInterval();

                if (interval <= 0)
                {
                    //インターバルの時間をリセット
                    interval = sketchManager.sketchInterval;

                    //本とクルクルの非表示
                    uIManager.SwitchingBookAndKurukuruDisplay(false);

                    //召喚できるようにする
                    sketchManager.summons = true;
                }

            }


            //Spaceが押されると、スケッチモードと通常モードを切り替える
            //summonsは、召喚可能の場合はOK
            //時間が止まってなかったらOK
            //スケッチアニメーション中出ない場合,OK
            if (Input.GetKeyDown(KeyCode.Space) && sketchManager.summons && !playerController.IsHanging && !isSketchAnimated)
            {

                SwitchingMarkersAndSketches(sketchMode);

                optionButton.SetActive(sketchMode);

                sketchMode = !sketchMode;

            }

        }
    }
    //マウスを押した時の、スクリーン座標をワールド座標に変換
    Vector3 MouseWorldPosition()
    {
        Vector3 position = Input.mousePosition;

        // ｚ軸を決める
        position.z = 10;

        //ｚを調整後にワールド座標に変換
        return Camera.main.ScreenToWorldPoint(position);

    }

    //モード切替
    public void SwitchingMarkersAndSketches(bool sketchMode)
    {
        if (sketchMode)
        {
            //スケッチアイコン、ライフ、フラワーキーアイコンの表示
            uIManager.UIDisplaySwitchingInSketchMode(true);

            //スケッチモード画面非表示
            uIManager.SketchModeSwitch(false);

            //スケッチモードを使えないようにして、マーカーは使える
            sketchManager.enabled = false;
            markerManager.enabled = true;

            //走りアニメーションを再生できるようにする
            if (playerController.Animator.GetBool("isSketching"))
            {
                playerController.Animator.SetBool("isSketching", false);
            }

            //プレイヤーを動けるようにする
            playerController.playerMove = true;

            playerManager.invincible = false;

        }
        else
        {
            //スペースキーを連打してもスケッチモードが開かないようにする処理
            isSketchAnimated = true;

            //アニメーションを再生してからスケッチモードを表示する
            StartCoroutine(SketchCoroutine());
        }
    }

    //フラワーキーを手に入れたときの処理
    public void GetFlowerKey()
    {
        //獲得した分減らす
        keysRemaining--;

        //フラワーキーアイコンUIの更新
        uIManager.FlowerKeyIconChange(keysRemaining);
    }

    //スケッチモード開始時の処理
    IEnumerator SketchCoroutine()
    {
        float endAnimationTime = 2.0f;  //アニメーションが終わる時間

        //マーカーを描いてる途中ならマーカーを終了させる
        markerManager.PaintFinish();

        //マーカーは使えないようにする
        markerManager.enabled = false;

        //スケッチ中にして、走りアニメーションを止める
        playerController.Animator.SetBool("isSketching", true);

        //プレイヤーの動きを止める
        playerController.playerMove = false;

        playerController.Animator.SetTrigger("sketchTrigger");
        
        //待機
        yield return new WaitForSeconds(endAnimationTime);  
        
        //スケッチモード画面表示
        uIManager.SketchModeSwitch(true);

        //スケッチモードを使えるようにする
        sketchManager.enabled = true;

        playerManager.invincible = true;

        //スケッチモード中に必要ないUIを非表示
        uIManager.UIDisplaySwitchingInSketchMode(false);

        isSketchAnimated = false;
    }
}

