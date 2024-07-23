using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSE : MonoBehaviour
{
    public AudioClip sound1;

    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(sound1, transform.position);
            //Debug.Log("“M‚ê‚½");
        }
    }
}
