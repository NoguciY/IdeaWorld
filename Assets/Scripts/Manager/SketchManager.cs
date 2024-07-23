using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchManager : MonoBehaviour
{
    [Header("再召喚までのインターバル秒数")]
    public float sketchInterval;

    //召喚が可能な場合はtrue (このスクリプトは、enabledでfalseになってしまうから、GameManagerで時間を計ってる)
    [HideInInspector] public bool summons = true;




    [Header("スケッチモードのTrailの参照")]
    [SerializeField] TrailRenderer trail;

    [Header("UI関連の参照")]
    [SerializeField] UIManager uIManager;

    [SerializeField] GameObject optionButton;

    [Header("オブジェクト生成関連の参照")]
    [SerializeField] SummonedObjGenerator generator;

    [Header("ゲームマネージャー")]
    [SerializeField] GameManager gameManager;

    [Header("プレイヤー関連の参照")]
    [SerializeField] PlayerController playerController;

    //紙の上だとtrueになる
    bool onPaper;

    //描ける状態かの確認
    bool sketchMaker = false;

    //お手本ボタンを押した時、チェックポイントの数を記録
    [HideInInspector] public int checkPoints;
    [HideInInspector] public int summonerNumber;


    void Update()
    {

        //紙の上か確認　(OnTriggerEnter2Dを使用)
        if (onPaper)
        {
            //右クリックで線を描き始める
            if (Input.GetMouseButtonDown(0))
            {
                SketchMarker(true);
            }

            //右クリックを離すと描くのを終了
            if (Input.GetMouseButtonUp(0))
            {
                SketchMarker(false);
                uIManager.ExampleDisplay(uIManager.IndexLastExample);

            }
        }
        else
        {
            //描くのを終了
            SketchMarker(false);
        }

    }

     //スケッチで描いてる途中でモードの切り替えがあった時の処理
    private void OnDisable()
    {
        SketchMarker(false);
    }

    //スケッチモードでのマーカーの　 ( ON / OFF )
    public void SketchMarker(bool switchMarker)
    {
        //描ける状態の切り替え
        sketchMaker = switchMarker;

        if (switchMarker)
        {
            trail.time = 10000f;
            trail.startWidth = 0.5f;
        }
        else
        {
            trail.time = 0f;
            trail.startWidth = 0f;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //紙の上ならスケッチ線が描ける
        if (collision.CompareTag("Paper"))
        {
            onPaper = true;
        }

        //マウスを押しながらチェックポイント通過
        if (collision.CompareTag("CheckPoint") && sketchMaker)
        {
            //チェックポイントを消していく
            collision.gameObject.SetActive(false);
            //チェックポイント通過した分だけ、減らしていく
            checkPoints--;

            if (checkPoints == 0)
            {
                //召喚成功後、スケッチモード画面を非表示にする
                uIManager.SketchModeSwitch(false);

                //モード切替
                gameManager.SwitchingMarkersAndSketches(true);
                
                //アニメーションした後に召喚する
                StartCoroutine(skillCoroutine());

                //召喚が出来ない状態に切り替え
                summons = false;

                //クルクルと本の表示
                uIManager.SwitchingBookAndKurukuruDisplay(true);
            }
        }
        else if (collision.CompareTag("OverflowCheck"))
        {
            //はみ出すとやり直し
            uIManager.ExampleDisplay(uIManager.IndexLastExample);
            SketchMarker(false);
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //紙の外ならスケッチ線描けない
        if (collision.CompareTag("Paper"))
        {
            onPaper = false;
        }
    }


    IEnumerator skillCoroutine()
    {
        float endAnimationTime = 1.5f;  //アニメーションが終わる時間

        //プレイヤーを動けなくする
        playerController.playerMove = false;

        //アニメーション再生
        playerController.Animator.SetTrigger("skillTrigger");
        playerController.Animator.SetBool("isActivatedSkill", true);

        yield return new WaitForSeconds(endAnimationTime);

        //召喚
        generator.SummonObj(summonerNumber);
        //Debug.Log("召喚");

        //プレイヤーを動けるようにする
        playerController.playerMove = true;

        //走りアニメーションを再生可能にする
        playerController.Animator.SetBool("isActivatedSkill", false);

        //オプションボタンを表示する
        uIManager.SwitchingOptionButtonDisplay(true);

        //スケッチアイコン、ライフ、フラワーキーアイコンの表示
        uIManager.UIDisplaySwitchingInSketchMode(true);
    }
}


