using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySE : MonoBehaviour
{
    public AudioClip sound;
    [Header("���Ԋu"), SerializeField] float chirpingInterval;

    //�J�E���g�_�E���p
    float count;

    [Header("Player�Q��"), SerializeField]
    Transform playerTransform;

    [Header("���͈�"), SerializeField]
    float range;

    private void Start()
    {
        count = chirpingInterval;
    }
    void Update()
    {
        //�v���C���[�ƓG�̊Ԃ̋����𑪂�
        float distance = Vector2.Distance(playerTransform.position, transform.position);

        //�v���C���[�ƓG�̊Ԃ̋��������͈͂ɓ�������
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