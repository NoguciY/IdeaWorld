using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("参照"),SerializeField]
    GameManager gameManager;

    void Start()
    {
        //gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        //キーの数だけインクリメントして、合計を求める
        gameManager.keysRemaining++;
    }

    //プレイヤーとの接触時の処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //フラワーキーゲット処理
            gameManager.GetFlowerKey();
           
        }
    }

}
