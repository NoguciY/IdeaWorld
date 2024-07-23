using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSketch : MonoBehaviour
{
    [Header("手に入れたいスケッチのButton")]
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
