using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ControlLever : MonoBehaviour
{
    //�h�A���J���Ă邩�̏�Ԋm�F�@�@�@true=�J���Ă�@�@false=���Ă�
    bool switching = false;

    [SerializeField,Header("�J���������̎Q��")]Door door;

    [SerializeField, Header("�J������̃��o�[�̃C���[�W")] Sprite sprite;
    [SerializeField]SpriteRenderer spriteRenderer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //���łɊJ���Ă�Ȃ烊�^�[��
        if (switching) return;


        if(collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("ButtonStone"))
        {
            spriteRenderer.sprite = sprite;
            switching = true;
            door.DoorOpen();
        }
    }


}
