using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStone : MonoBehaviour
{
    [SerializeField,Header("�J�������h�A�̎Q��")] Door door;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ButtonStone"))
        {
            door.DoorOpen();
        }
    }
}
