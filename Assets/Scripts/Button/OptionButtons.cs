using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionButtons : MonoBehaviour
{
    public GameObject titleButton;
    public GameObject returnButton;
    public GameObject tutorialButton;
    public GameObject checkPointButton;
    public GameObject optionButtonsPanel;

    public GameObject tutorialImage;

    //オプションボタン押した時の処理
    public void Option_Button()
    {
        //時間を止める
        Time.timeScale = 0;
        Debug.Log("止まった");


        //オプションボタンは非表示
        gameObject.SetActive(false);


        //色々なボタンを表示する
        optionButtonsPanel.SetActive(true);
       
    }



    //チュートリアル画面表示する
    public void TutorialDisplay()
    {
        tutorialImage.SetActive(true);

        optionButtonsPanel.SetActive(false);
    }



    //ゲームに戻る
    public void return_Button()
    {
        Time.timeScale = 1;
        Debug.Log("時は動き出す...");

        gameObject.SetActive(true);

        optionButtonsPanel.SetActive(false);
    }
    
    public void Title_Button()
    {
        Cursor.visible = true;
        Time.timeScale = 1;
        //optionButtonsPanel.SetActive(true);
        gameObject.SetActive(true);
        SceneManager.LoadScene("TitleScene");
    }

    // オプションボタンを画面に表示の切り替え
    public void OpsionButtonsPanelSwitching(bool s)
    {
        optionButtonsPanel.SetActive(s);
    }
}
