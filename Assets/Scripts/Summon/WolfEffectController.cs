using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//狼の吠えるアニメーション時に表示する画像に関するクラス
//いつもは画像のα値を最小にして
//吠えるときにα値を最大にする
//表示した後、徐々に透明度を下げて非表示にする

public class WolfEffectController : MonoBehaviour
{
    [SerializeField] float displayedImageTime = 1.2f;   //画像を表示させる時間

    const int maxColorA = 255;                          //最大α値
    const int minColorA = 0;                            //最低α値
    const float untilBarkWaitingTime = 0.5f;            //吠えるまでの待ち時間
    bool isBarking = false;                             //狼が吠えたか
    CircleCollider2D circleCollider;                    //エフェクトが表示している間有効
    GameObject[] stageObjects = new GameObject[50];     //エフェクトに当たったオブジェクト(地面と川)を格納する
    int stageObjectCounter = 0;                         //OrderInLayerを下げるオブジェクトの数をカウント

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBarking)
        {
            isBarking = false;
            StartCoroutine(ControllImageAlfa());
        }
    }

    //α値を変動させる
    IEnumerator ControllImageAlfa()
    {
        yield return new WaitForSeconds(untilBarkWaitingTime);

        //画像を表示する
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, maxColorA);
        
        //コライダーを有効にする
        circleCollider.enabled = true;

        yield return new WaitForSeconds(displayedImageTime);

        //画像を透過させる
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, minColorA);
        
        //エフェクトが消えたらステージオブジェクトを前に出す
        for (int j = 0; j < stageObjectCounter; j++) 
        {
            stageObjects[j].GetComponent<SpriteRenderer>().sortingOrder = 65;
            stageObjects[j] = null;
        }

        stageObjectCounter = 0;

        //コライダーを無効にする
        circleCollider.enabled = false;
    }

    public void ActiveControllImageAFlag()
    {
        isBarking = true;
    }

    //エフェクトのOrderInLayerを地面と川のオブジェクトより前にする
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")||
            collision.gameObject.CompareTag("River"))
        {
            stageObjects[stageObjectCounter] = collision.gameObject;
            stageObjects[stageObjectCounter].GetComponent<SpriteRenderer>().sortingOrder = 15;
            stageObjectCounter++;
        }
    }
}
