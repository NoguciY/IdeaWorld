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
        //Component���擾
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //���X�|�[���n�_�̏ꏊ�̒���
        Vector3 offset = new Vector3(0f, -0.6f, 0);

        //�`�F�b�N�|�C���g���L�^����
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            AudioSource.PlayClipAtPoint(sound1, transform.position);

            particle.SetActive(true);

            if (player != null)
            {
                player.checkPoint = transform.position+offset;
                Debug.Log("�`�F�b�N�|�C���g");
            }

            //�ʉ߂����`�F�b�N�|�C���g�͓����蔻�����
            Destroy(gameObject.GetComponent<BoxCollider2D>());

            
        }
    }
}
