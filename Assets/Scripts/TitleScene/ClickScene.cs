using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickScene : MonoBehaviour
{
    [Header("クリック出来るようになるまでの時間設定"), SerializeField]
    float time;

    //カウントダウン用変数
    float t;

    [Header("クリックパネル"), SerializeField]
    GameObject ClickPanel;

    [Header("クリック後に遷移したいシーンの名前"), SerializeField]
    string seneName;

    void Start()
    {
        //時間を設定
        t = time;
    }

    void Update()
    {

        if(t>0)
        {
            //カウントダウン
            t -= Time.deltaTime;
        }
        else if(t<0)
        {
            //画僧表示
            ClickPanel.SetActive(true);

            //クリックでタイトルシーンに遷移
            if(Input.GetMouseButtonDown(0))
            {
                //マウスカーソルを表示
                Cursor.visible = true;

                SceneManager.LoadScene(seneName);
            }
        }

    }
}
