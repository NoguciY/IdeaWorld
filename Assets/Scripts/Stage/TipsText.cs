using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

//看板に当たるとsキー押下画像が表示、sキーを押すとヒントが出現
//もう一度押すとヒントを閉じる
//画像が非表示になった瞬間に下の処理で表示されるため
//キーを入力した後にそのキーに対して処理をしたかを判別する変数を用意する

public class TipsText : MonoBehaviour
{
    public AudioClip sound1;

    [SerializeField] GameObject hintImage;                  //ヒント画像
    [SerializeField] GameObject hintKeyImage;               //ヒントキー画像
    [SerializeField] PlayerController playerController;     //プレイヤーがキー入力したかを確認する
    [SerializeField] RectTransform canvasRectTrans;         //ヒント押下画像の親のキャンバス
    [SerializeField] Camera targetCamera;                   //オブジェクトを映すカメラ

    AudioSource audioSource;                                //SE
    bool isHittingHintBoard = false;                        //看板に当たっているか
    bool isPressedHintKey = false;                          //ヒントキーが押されたか
    bool isDisplayingHint = false;                          //ヒントが表示されているか
    bool isPerformedHintKeyInputProcessing = false;         //ヒントキー入力処理を行ったか
    int hintImageAlfaValue = 0;                             //ヒント画像のα値
    int hintKeyImageAlfaValue = 0;                          //ヒントキー画像のα値

    const int maxAlfaValue = 255;                           //最大α値
    const int minAlfaValue = 0;                             //最小α値

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //ヒントキー判定の更新
        isPressedHintKey = playerController.IsPressedHintKey;

        //キーが押下された かつ ヒントが表示されている場合
        if (isPressedHintKey && isDisplayingHint)
        {
            //キー入力処理を行った
            isPerformedHintKeyInputProcessing = true;

            //ヒントを非表示
            isDisplayingHint = false;
            hintImageAlfaValue = minAlfaValue;
            hintImage.GetComponent<Image>().color = new Color(255, 255, 255, hintImageAlfaValue);
        }

        //看板に当たっている かつ キー押下された かつ ヒントが表示されていない場合
        if (isHittingHintBoard && isPressedHintKey && !isDisplayingHint)
        {
            //キー入力処理をこのフレームで行っていない場合
            if (!isPerformedHintKeyInputProcessing)
            {
                //ヒントキー押下画像を非表示
                hintKeyImageAlfaValue = minAlfaValue;
                hintKeyImage.GetComponent<Image>().color = new Color(255, 255, 255, hintKeyImageAlfaValue);

                //SE再生
                AudioSource.PlayClipAtPoint(sound1, transform.position, 1f);

                //ヒントを表示
                isDisplayingHint = true;
                hintImageAlfaValue = maxAlfaValue;
                hintImage.GetComponent<Image>().color = new Color(255, 255, 255, hintImageAlfaValue);
            }
            else { isPerformedHintKeyInputProcessing = false; }
            }
    }

    //プレイヤーがヒントポイントに当たった瞬間、ヒントを表示
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHittingHintBoard = true;

            //ヒント看板の真上にヒント押下画像を表示させる
            Vector2 hintBoardPos = (Vector2)transform.position + Vector2.up * 2.0f;     //ヒント看板の座標
            var hintKeyImagePos = Vector2.zero;                                         //ヒント押下画像の座標

            //(ワールド）座標をスクリーン座標に変換する
            hintBoardPos = targetCamera.WorldToScreenPoint(hintBoardPos);

            //スクリーン座標をRectTransform座標に変換する
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, hintBoardPos, null, out hintKeyImagePos);

            hintKeyImage.GetComponent<RectTransform>().localPosition = hintKeyImagePos;

            //ヒントキー押下画像を表示
            hintKeyImageAlfaValue = maxAlfaValue;
            hintKeyImage.GetComponent<Image>().color = new Color(255, 255, 255, hintKeyImageAlfaValue);
        }
    }

    //プレイヤーがヒントポイントから離れた瞬間、ヒントを非表示
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHittingHintBoard = false;
            
            //ヒントキー画像が表示されている場合
            if (hintKeyImageAlfaValue != minAlfaValue)
            {
                //ヒントキー画像を非表示
                hintKeyImageAlfaValue = minAlfaValue;
                hintKeyImage.GetComponent<Image>().color = new Color(255, 255, 255, hintKeyImageAlfaValue);
            }
        }
    }
}
