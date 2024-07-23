using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//狼の吠えるアニメーション時に表示する画像に関するクラス
//いつもは画像のα値を最小にして
//吠えるときにα値を最大にする
//表示した後、徐々に透明度を下げて非表示にする

public class EnemyEffectController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] float untilBarkWaitingTime = 0.7f; //吠えるまでの待ち時間
    [SerializeField] float displayedImageTime = 3.0f;   //画像を表示させる時間

    const int maxColorA = 255;                          //最大α値
    const int minColorA = 0;                            //最低α値
    bool isBarkingWolf = false;                         //狼が吠えたかどうか

    void Update()
    {
        //狼がスキル発動した場合、エフェクトを表示する
        if (enemyController.activeWolfSkill && !isBarkingWolf)
        {
            isBarkingWolf = true;
            StartCoroutine(ControllImageAlfa());
        }

        //狼がスキル発動してない場合、吠えていない
        if (!enemyController.activeWolfSkill && isBarkingWolf)
        {
            isBarkingWolf = false;
        }
    }
    
    //α値を変動させる
    IEnumerator ControllImageAlfa()
    {
        yield return new WaitForSeconds(untilBarkWaitingTime);

        //画像を表示する
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, maxColorA);

        yield return new WaitForSeconds(displayedImageTime);

        //画像を透過させる
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, minColorA);
    }
}
