using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flashing : MonoBehaviour
{
    [Header("点滅させたいイメージコンポーネント"), SerializeField]
    Image image;

    [Header("変化させたい色の設定"), SerializeField]
    Color changeColor;

    [Header("設定の色に変わるまでの時間"), SerializeField]
    float changeTime;
    [Header("元の色に戻るまでの時間"), SerializeField]
    float returnTime;

    //元の色を入れておく
    Color originalColor;

    //時間
    float time;

    //割合
    float ratio;

    // 変化中かどうかのフラグ
    bool isChanging = true;




    private void Start()
    {
        //元の色を代入
        originalColor = image.color;
    }
    void Update()
    {
        //時間を計る
        time += Time.deltaTime;

        if (isChanging)
        {
            //時間/変更までの時間　　の割合を出す
            ratio = time / changeTime;

            //　ratioを(0～１)の範囲に調整
            ratio = Mathf.Clamp01(ratio);

            //割合を使って色を変更
            image.color = originalColor + (changeColor - originalColor) * ratio;
        }
        else
        {
            //時間/変更までの時間　　の割合を出す
            ratio = time / returnTime;

            //　ratioを(0～１)の範囲に調整
            ratio = Mathf.Clamp01(ratio);

            //割合を使って色を変更
            image.color = changeColor + (originalColor-changeColor) * ratio;

        }

        if (ratio >= 1)
        {
            //色を変えるのを反転
            isChanging = !isChanging;

            //時間をリセット
            time = 0;
        }


    }
}
