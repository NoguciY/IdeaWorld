using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilDestory : MonoBehaviour
{
    public AudioClip sound1;
    AudioSource audioSource;

    void Start()
    {
        //Component‚ðŽæ“¾
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LineParent"))
        {
            AudioSource.PlayClipAtPoint(sound1, transform.position);
            Destroy(gameObject);
        }
    }
}