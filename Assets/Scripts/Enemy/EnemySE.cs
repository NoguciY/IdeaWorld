using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySE : MonoBehaviour
{
    public AudioClip sound;
    [Header("Β­Τu"), SerializeField] float chirpingInterval;

    //JEg_Ep
    float count;

    [Header("PlayerQΖ"), SerializeField]
    Transform playerTransform;

    [Header("Β­ΝΝ"), SerializeField]
    float range;

    private void Start()
    {
        count = chirpingInterval;
    }
    void Update()
    {
        //vC[ΖGΜΤΜ£πͺι
        float distance = Vector2.Distance(playerTransform.position, transform.position);

        //vC[ΖGΜΤΜ£ͺΒ­ΝΝΙόΑ½η
        if (distance <= range)
        {
            count -= Time.deltaTime;
            //Debug.Log(count);
            if (count <= 0)
            {
                count = chirpingInterval;
                AudioSource.PlayClipAtPoint(sound, transform.position);
            }
        }
    }
}