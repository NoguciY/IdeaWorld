using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class UIManager : MonoBehaviour
{
    [Header("スケッチ画面")]
    [SerializeField] GameObject sketchModeBack;

    [Header("スケッチのお手本を表示するボタン")]
    [SerializeField] GameObject ExampleButtons;

    [Header("スケッチお手本")]
    [SerializeField] UnityEngine.UI.Image[] sketchExample;

    [Header("スケッチお手本チェックポイント(種類ごと)")]
    [SerializeField] GameObject[] checkPoint;



    [SerializeField] GameObject optionButton;



    [Header("召喚インターバルのText"), SerializeField]
    TextMeshProUGUI intervalText;
    [Header("インターバルのクルクル"), SerializeField]
    GameObject kurukuru;
    [Header("インターバルの本"), SerializeField]
    Image book;



    [Header("フラワーキーアイコン（完成形(0)～未完成(6)の順番でいれてね）")]
    [SerializeField] Sprite[] flowerSprite;

    [Header("フラワーキーアイコンのオブジェクト")]
    [SerializeField] Image flowerKeyIcon;

    [Header("フラワーキー完成時に表示する画像"), SerializeField]
    GameObject completeImage;

    [Header("フラワーキー完成テキストの移動スピード"), SerializeField]
    float speed;

    [Header("フラワーキー完成テキスト表示位置の高さ"), SerializeField]
    float flowerIconHeight;




    [Header("LifeUIPanelの参照"), SerializeField]
    GameObject lifeUIPanel;

    [Header("SketchIconの参照"), SerializeField]
    GameObject SketchIcon;

    [Header("召喚スペース足りないTextの親"), SerializeField]
    GameObject adviceTexts;

    //テキストの参照を入れておく
    TextMeshProUGUI[] texts;

    //元の色を覚えておく
    Color[] originalColors;

    [Header("透明になって消えるまでの時間"), SerializeField]
    float adviceTransparencyTime;




    [Header("マネージャー系参照")]
    [SerializeField] SketchManager sketchManager;
    [SerializeField] SummonedObjGenerator generator;
    [SerializeField] GameManager gameManager;



    //最後に表示したお手本のインデックス
    //初期値   (-1)
    [HideInInspector] public int IndexLastExample = -1;


    private void Start()
    {
        //子オブジェクトのTextの参照を配列に入れる
        texts = adviceTexts.GetComponentsInChildren<TextMeshProUGUI>();

        originalColors = new Color[texts.Length];

        for (int i = 0; i < texts.Length; i++)
        {
            //元の色を記録
            originalColors[i] = texts[i].color;
        }
    }

    //本とクルクルを表示
    public void SwitchingBookAndKurukuruDisplay(bool s)
    {
        if (s)
        {
            //クルクル表示
            kurukuru.SetActive(true);

            //本の色を変える
            Color color = new Color(255f / 255, 255f / 255f, 255f / 255f, 80f / 255f);
            book.color = color;
        }
        else
        {
            //クルクル非表示
            kurukuru.SetActive(false);

            //本を明るく
            Color color = new Color(255f / 255, 255f / 255f, 255f / 255f, 255f / 255f);
            book.color = color;

        }


    }
    public void DisplayInterval()
    {
        //インターバルテキスト更新
        intervalText.text = Mathf.Floor(gameManager.interval).ToString();

        //クルクルをクルクル
        kurukuru.GetComponent<RectTransform>().Rotate(0f, 0f, -360f * Time.deltaTime, Space.World);


        if (gameManager.interval <= 0)
        {
            //インターバルテキストを非表示
            intervalText.text = "";
        }

    }



    //Space押されたときの処理
    public void SketchModeSwitch(bool modeSwitch)
    {

        //スケッチ画面表示  (紙と背景)
        sketchModeBack.SetActive(modeSwitch);
        Debug.Log("modeSeitch:   " + modeSwitch);


        //お手本ボタン表示
        ExampleButtons.SetActive(modeSwitch);


        //前のお手本がある場合は非表示にする
        if (IndexLastExample >= 0)
        {
            //前回表示されていたお手本を消す
            sketchExample[IndexLastExample].enabled = false;
            //前回のチェックポイントの非表示
            checkPoint[IndexLastExample].transform.parent.gameObject.SetActive(false);
        }
    }



    //お手本表示ボタン押した時の処理
    public void ExampleDisplay(int index)
    {
        if(index <= -1)
        {
            return;
        }

        //召喚スペースの確認　ダメならreturnする
        if (generator.CheckSummoningSpace(index) == false)
        {
            StartCoroutine(TransparencyText(index, adviceTransparencyTime));
            return;
        }



        //前のお手本がある場合は非表示にする
        if (IndexLastExample >= 0)
        {
            //前回表示されていたお手本を消す
            sketchExample[IndexLastExample].enabled = false;
            //前回のチェックポイントの非表示
            checkPoint[IndexLastExample].transform.parent.gameObject.SetActive(false);
        }

        //チェックポイントを全てアクティブにして、チェックポイントの数をmouseControllerに代入する
        sketchManager.checkPoints = ResetCheckPoints(index);

        //召喚獣の番号を代入
        sketchManager.summonerNumber = index;

        //新しいお手本
        sketchExample[index].enabled = true;

        //チェックポイントの表示
        checkPoint[index].transform.parent.gameObject.SetActive(true);

        //最後に表示したお手本のインデックスを記録
        IndexLastExample = index;

    }


    //チェックポイント表示の切り替え
    //戻り値は、チェックポイントの数
    int ResetCheckPoints(int index)
    {
        //チェックポイントをカウント
        int checkPoints = 0;


        //チェックポイントを全てアクティブにする
        foreach (Transform tr in checkPoint[index].transform)
        {
            tr.gameObject.SetActive(true);

            checkPoints++;
        }
        return checkPoints;
    }

    //フラワーキーアイコンを変更する
    public void FlowerKeyIconChange(int keyCount)
    {
        //イメージの変更   (+1しているのは、インデックスの調整)
        flowerKeyIcon.GetComponent<UnityEngine.UI.Image>().sprite = flowerSprite[keyCount + 1];


        //フラワーキーを全て集めた時の処理
        if (keyCount == 0)
        {
            StartCoroutine(FlowerKeyComplete());
        }
    }

    //フラワーキー完成時の処理
    //テキストを表示して、花を鍵に変える
    IEnumerator FlowerKeyComplete()
    {
        //フラワーキー完成の画像をアクティブにする
        completeImage.SetActive(true);
        RectTransform image = completeImage.GetComponent<RectTransform>();

        //画面下からflowerIconHeightの高さまで移動させる
        while (image.anchoredPosition.y < flowerIconHeight)
        {
            image.anchoredPosition += Vector2.up * speed;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        //画像表示３秒後に、画像を非表示にして、フラワーキーアイコンを鍵の画像に変更する
        image.gameObject.SetActive(false);
        flowerKeyIcon.GetComponent<UnityEngine.UI.Image>().sprite = flowerSprite[0];
    }

    //オプションボタンの表示の切り替え
    public void SwitchingOptionButtonDisplay(bool s)
    {
        optionButton.SetActive(s);
    }


    public IEnumerator TransparencyText(int adviceNumber, float transparencyTime)
    {

        //テキストを表示
        texts[adviceNumber].enabled = true;


        //元の色に戻す
        texts[adviceNumber].color = originalColors[adviceNumber];



        //第二引数の秒数をかけて徐々に透明にする
        float t = texts[adviceNumber].color.a / transparencyTime / 100;

        while (texts[adviceNumber].color.a >= 0)
        {
            texts[adviceNumber].color -= new Color(0f, 0f, 0f, t);
            yield return new WaitForSeconds(t);
        }



        //透明にした後は、非表示にする
        texts[adviceNumber].enabled = false;


    }

    /// <summary>
    /// スケッチモード中のUI表示の切り替え
    /// </summary>
    public void UIDisplaySwitchingInSketchMode(bool s)
    {
        //LifeUIの表示切り替え
        lifeUIPanel.gameObject.SetActive(s);

        //フラワーキーアイコンの表示切り替え
        flowerKeyIcon.transform.parent.gameObject.SetActive(s);

        //スケッチアイコンの表示切り替え
        SketchIcon.SetActive(s);
    }
}
