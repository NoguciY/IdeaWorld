using UnityEngine;
using UnityEngine.UI;

//ライフUIをプレイヤーのHPに応じて更新する関数


public class LifeUIManager : MonoBehaviour
{
    public GameObject player;       //プレイヤー
    public GameObject[] playerLifeUI = new GameObject[3];   //ライフUIx3
    Color[] playerLifeUIColor = new Color[3];               //ライフの画像の処理をするため
    int playerLife;                 //プレイヤーのライフ
    int preLife;                    //前のライフを保存する
    PlayerManager playerManager;    //現在のHPを取得するため
    RectTransform myRectTransform;  //ライフUIパネルのスクリーン座標を使う
    int spacePushCount = 0;         //スペースキーを押した回数

    // Start is called before the first frame update
    void Start()
    {
        playerManager = player.GetComponent<PlayerManager>();
        myRectTransform = GetComponent<RectTransform>();

        //ライフUI3つのimageを格納
        for (int i = 0; i < 3; i++)
        {
            playerLifeUIColor[i] = playerLifeUI[i].GetComponent<Image>().color;
        }

        preLife = playerManager.GetLife();

    }

    // Update is called once per frame
    void Update()
    {
        //UIをスクリーン座標の同じ座標に固定する
        myRectTransform.position = myRectTransform.position;

        //UIを更新する
        LifeUIUpdate();

        //スケッチ画面表示中は透明度を下げる
        LowLifeAlphaDisplayedSketchScreen();
    }

    //UIを現在のライフに合わせて更新する関数
    void LifeUIUpdate()
    {
        //プレイヤーの現在のライフを取得
        playerLife = playerManager.GetLife();

        //ライフが変わった場合
        if (playerLife != preLife)
        {
            //ライフが3の場合
            if (playerLife == 3)
            {
                preLife = playerLife;

                for (int i = 0; i < 3; i++)
                {
                    //ライフUIは3つとも明瞭
                    playerLifeUIColor[i].a = 1.0f;
                    playerLifeUI[i].GetComponent<Image>().color = playerLifeUIColor[i];
                }
            }
            //ライフが2の場合
            else if (playerLife == 2)
            {
                preLife = playerLife;

                //右のライフUIを半透明にする
                playerLifeUIColor[0].a = 0.5f;
                playerLifeUI[0].GetComponent<Image>().color = playerLifeUIColor[0];
            }
            //ライフが1の場合
            else if (playerLife == 1)
            {
                preLife = playerLife;

                //中央のライフUIを半透明にする
                playerLifeUIColor[1].a = 0.5f;
                playerLifeUI[1].GetComponent<Image>().color = playerLifeUIColor[1];
            }
            //ライフが0の場合
            else if (playerLife == 0)
            {
                preLife = playerLife;

                //左のライフUIを半透明にする
                playerLifeUIColor[2].a = 0.5f;
                playerLifeUI[2].GetComponent<Image>().color = playerLifeUIColor[2];
            }
        }
    }

    //スケッチ画面が表示されている間は透明度を下げる関数
    void LowLifeAlphaDisplayedSketchScreen()
    {
        //スペースキーを押した場合
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //カウントをプラス
            spacePushCount++;

            if (spacePushCount == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    //ライフUIを3つとも透明度を下げる
                    playerLifeUIColor[i].a -= 0f;
                    playerLifeUI[i].GetComponent<Image>().color = playerLifeUIColor[i];
                    Debug.Log(spacePushCount);
                }
            }
            else
            {
                //カウントをリセット
                spacePushCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    //ライフUIを3つとも元の透明度に戻す
                    playerLifeUIColor[i].a += 0f;
                    playerLifeUI[i].GetComponent<Image>().color = playerLifeUIColor[i];
                    Debug.Log(spacePushCount);
                }
            }
        }
    }
}