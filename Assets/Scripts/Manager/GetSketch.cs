using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSketch : MonoBehaviour
{
    [Header("��ɓ��ꂽ���X�P�b�`��Button")]
    [SerializeField] GameObject button;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            button.SetActive(true);
        }
    }
}
