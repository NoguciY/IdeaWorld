using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureChest : MonoBehaviour
{
    [SerializeField]GameManager gameManager;

    [Header("UIの宝箱のアニメーションのオブジェクト"),SerializeField]GameObject Takaraanimator;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(gameManager.keysRemaining==0)
            {
                Debug.Log("クリア");
                //プレイヤーの動きを止める
                collision.GetComponent<PlayerController>().playerMove = false;

                //アクティブにすして、アニメーションを再生する
                Takaraanimator.gameObject.SetActive(true);

                //クリアのフラグ
                ClearTheGame.clearTheGame.GameClear = true;
            }
            else
            {
                Debug.Log("鍵が必要");
            }
        }
    }

}
