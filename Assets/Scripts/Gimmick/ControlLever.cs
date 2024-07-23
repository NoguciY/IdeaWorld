using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ControlLever : MonoBehaviour
{
    //ドアが開いてるかの状態確認　　　true=開いてる　　false=閉じてる
    bool switching = false;

    [SerializeField,Header("開けたい扉の参照")]Door door;

    [SerializeField, Header("開いた後のレバーのイメージ")] Sprite sprite;
    [SerializeField]SpriteRenderer spriteRenderer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //すでに開いてるならリターン
        if (switching) return;


        if(collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("ButtonStone"))
        {
            spriteRenderer.sprite = sprite;
            switching = true;
            door.DoorOpen();
        }
    }


}
