using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public AudioClip sound1;
    AudioSource audioSource;
    public GameObject particle;

    void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //リスポーン地点の場所の調整
        Vector3 offset = new Vector3(0f, -0.6f, 0);

        //チェックポイントを記録する
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            AudioSource.PlayClipAtPoint(sound1, transform.position);

            particle.SetActive(true);

            if (player != null)
            {
                player.checkPoint = transform.position+offset;
                Debug.Log("チェックポイント");
            }

            //通過したチェックポイントは当たり判定消す
            Destroy(gameObject.GetComponent<BoxCollider2D>());

            
        }
    }
}
