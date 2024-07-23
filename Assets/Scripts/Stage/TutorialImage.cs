using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class TutorialImage : MonoBehaviour
{ 
    [Header("チュートリアルの画像(0から順番に入れてね)"), SerializeField]
    Sprite[] tutorialImages;
    public int index=0;

    [Header("変更したいImageの参照を入れてね"),SerializeField]
    Image tutorialImage;

    [SerializeField] GameObject optionButoon;


    private void Start()
    {
        //配列に入れられているSpriteの０番目をImageに入れる
        tutorialImage.sprite = tutorialImages[0];
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ImageInForeground();
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            NextImage();
        }

    }
    void NextImage()
    {
        index++;
        CheckIndex();
        tutorialImage.sprite = tutorialImages[index];
    }
    void ImageInForeground()
    {
        index--;
        CheckIndex();
        tutorialImage.sprite = tutorialImages[index];

    }
    void CheckIndex()
    {
        if (index < 0)
        {
            index = 0;
        }
        else if (index > tutorialImages.Length-1)
        {
            //インデックス用変数は0に戻しておく
            index = 0;

            //閉じる
            TutorialClose();

        }
    }

    //ボタンで閉じるとき用
    public void TutorialClose()
    {
        //最初の画像に戻しておく
        tutorialImage.sprite = tutorialImages[0];

        //非表示にする
        gameObject.SetActive(false);

        //Optionボタンは、表示
        optionButoon.SetActive(true);

        //時間を進むようにする
        Time.timeScale = 1;
    }


}
