using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySE : MonoBehaviour
{
    public AudioClip sound;
    [Header("鳴く間隔"), SerializeField] float chirpingInterval;

    //カウントダウン用
    float count;

    [Header("Player参照"), SerializeField]
    Transform playerTransform;

    [Header("鳴く範囲"), SerializeField]
    float range;

    private void Start()
    {
        count = chirpingInterval;
    }
    void Update()
    {
        //プレイヤーと敵の間の距離を測る
        float distance = Vector2.Distance(playerTransform.position, transform.position);

        //プレイヤーと敵の間の距離が鳴く範囲に入ったら
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