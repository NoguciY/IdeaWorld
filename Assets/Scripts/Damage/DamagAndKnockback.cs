using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagAndKnockback : MonoBehaviour
{
    [Header("�v���C���[�Ƃ̐ڐG��A���I�u�W�F�N�g�������ǂ���")]
    [SerializeField] bool destroy;

    [Header("���ԏ��ł̗L��")]
    [SerializeField] bool timecount;

    [Header("���ł܂ł̎���")]
    [SerializeField] float countDownTime;
    float time;

    [Header("�m�b�N�o�b�N�̗L��")]
    [SerializeField] bool knockback;

    [Header("�m�b�N�o�b�N�̈З�")]
    [SerializeField] float knockbackPower;

    [Header("�_���[�W�̗L��")]
    [SerializeField] bool damage;

    [Header("�U����")]
    [SerializeField] int attackPower;

    private void Start()
    {
        time = countDownTime;
    }
    void Update()
    {
        if (timecount)
        {
            //�J�E���g�_�E���łO�ɂȂ�Ə�����
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerManager>().invincible)
            {
                Destroy(gameObject);
                return;
            }


            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();


            if (player == null) return;


            if (damage)
            {
                player.PlayerDamage(attackPower);
            }

            if (knockback)
            {
                //�v���C���[�̃m�b�N�o�b�N��true�ɂ���
                player.GetComponent<PlayerController>().knockback = true;

                Vector2 vec = Vector2.left * knockbackPower;
                if (transform.position.x < player.transform.position.x)
                {
                    vec *= -1;
                }

                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb == null) return;

                rb.AddForce(vec, ForceMode2D.Impulse);
            }

            if (destroy)
            {
                Destroy(gameObject);
            }

        }
        else if (collision.gameObject.CompareTag("Wolf"))
        {
            Destroy(gameObject);
        }
    }

}
