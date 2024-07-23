using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagAndKnockback : MonoBehaviour
{
    [Header("プレイヤーとの接触後、自オブジェクト消すかどうか")]
    [SerializeField] bool destroy;

    [Header("時間消滅の有無")]
    [SerializeField] bool timecount;

    [Header("消滅までの時間")]
    [SerializeField] float countDownTime;
    float time;

    [Header("ノックバックの有無")]
    [SerializeField] bool knockback;

    [Header("ノックバックの威力")]
    [SerializeField] float knockbackPower;

    [Header("ダメージの有無")]
    [SerializeField] bool damage;

    [Header("攻撃力")]
    [SerializeField] int attackPower;

    private void Start()
    {
        time = countDownTime;
    }
    void Update()
    {
        if (timecount)
        {
            //カウントダウンで０になると消える
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
                //プレイヤーのノックバックをtrueにする
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
