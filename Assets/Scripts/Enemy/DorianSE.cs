using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DorianSE : MonoBehaviour
{
    public AudioClip sound1;

    void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(sound1, transform.position);

        }
    }
}
