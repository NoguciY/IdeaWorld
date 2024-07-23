using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

/*
 �������ꂽ��؋󂷂�
 �v���C���[�Ɠ�����ƁA�v���C���[�͂Ԃ牺�����ԂɂȂ�
 ���D�͕��V����
 ���V���Ă���Ƃ��́A���C���̏d���ɂ���ď㏸�͂��ς��
 ���̃I�u�W�F�N�g�ɓ�����Ƒޏꂷ��
 �Ԃ牺�����Ԃ̃v���C���[�͍��E�̈ړ����x��������A�W�����v���ł��Ȃ��Ȃ�
 �v���C���[���|���ꂽ��Ԃ牺�����Ԃ���������
*/

public class BalloonController : SummonedObjController
{
    public float AxisH { get; set; }
    public float speedX = 0;        //�������̑��x

    [SerializeField] private float slightlyIncreasedBuoyancy = 1.4f;    //���D�̕���(������)
    [SerializeField] private float fairlyIncreasedBuoyancy = 2.1f;      //���D�̕���(������)
    [SerializeField] private float greatlyIncreasedBuoyancy = 3.5f;     //���D�̕���(�呝��)

    float sppedY;                       //�c�����̑��x
    float scaleX;                       //���X�P�[��
    bool checkHitPlayer = false;        //�v���C���[�Ɠ���������
    bool checkHitObj = false;           //�I�u�W�F�N�g�ɓ���������
    bool checkPlayer = false;           //�v���C���[�ɕ�������������
    bool isHittingCollider = false;     //�v���C���[���I�u�W�F�N�g�ƏՓ˂�����
    Rigidbody2D rb;
    Rigidbody2D playerRb;
    BoxCollider2D boxCollide;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        sppedY = speed;
        rb = GetComponent<Rigidbody2D>();
        boxCollide = GetComponent<BoxCollider2D>();
        scaleX = transform.localScale.x;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //������ς���
        if (AxisH > 0)
        {
            transform.localScale = new Vector2(scaleX, transform.localScale.y);
        }
        else if (AxisH < 0)
        {
            transform.localScale = new Vector2(-scaleX, transform.localScale.y);
        }

        //�v���C���[�ƃI�u�W�F�N�g�Ƃ̓����蔻��𒲂ׂ�
        CheckPlayersCollider();

        //���D��j������
        DestroySummonedObj();
    }

    private void FixedUpdate()
    {
        if (checkHitPlayer)
        {
            //�v���C���[�ɓ��������ꍇ�A�X�L������
            SummonedObjSkill();
        }
    }

    protected override void SummonedObjSkill()
    {
        if (rb.isKinematic)
        {
            //���D�̕������Z��L����
            rb.isKinematic = false;
        }

        //�ړ�����
        rb.velocity = new Vector2(speedX, speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //���ȊO�ɓ��������ꍇ(�����I�u�W�F�N�g�A�V��A�n�ʁA�G)
        if (collision.gameObject.CompareTag("DeadObject")   ||
            collision.gameObject.CompareTag("Ceiling")      ||
            collision.gameObject.CompareTag("Ground")       ||
            collision.gameObject.CompareTag("Enemy")        ||
            collision.gameObject.CompareTag("Enemies"))
        {
            checkHitObj = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�v���C���[�ɓ��������ꍇ
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            playerController = collision.gameObject.GetComponent<PlayerController>();
            
            //�v���C���[�̑��x��0�ɂ���
            playerRb.velocity = Vector2.zero;

            //�v���C���[�̏d�͂��Ȃ���  
            playerRb.gravityScale = 0;

            //�v���C���[���������̏ꍇ�A���D���������ɂ���
            if (collision.gameObject.transform.localScale.x < 0)
            {
                this.transform.localScale *= new Vector2(-1.0f, 1.0f);
            }

            //�v���C���[�̐e�𕗑D(����)�ɂ���
            collision.gameObject.transform.parent = gameObject.transform;

            //�v���C���[�ɓ��������t���O�𗧂Ă�
            checkHitPlayer = true;      
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //�����������Ă���ԁA���͂���������
        if (collision.gameObject.CompareTag("LineParent"))
        {
            float lineMass = collision.gameObject.GetComponent<Rigidbody2D>().mass;
            //Debug.Log("Mass" + lineMass);

            if (lineMass > 1.0f && lineMass <= 3.0f) 
            {
                speed = slightlyIncreasedBuoyancy;
            }            
            else if (lineMass > 3.0f && lineMass <= 18.0f)
            {
                speed = fairlyIncreasedBuoyancy;
            }           
            
            else if (lineMass > 18.0f)
            {
                speed = greatlyIncreasedBuoyancy;
            }           
        }               
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //�������ꂽ�u�ԁA���͂����ɖ߂�
        if (collision.gameObject.CompareTag("LineParent"))
        { speed = sppedY; }
    }

    protected override void DestroySummonedObj()
    {
        //�v���C���[�Ɛ��ȊO�̃I�u�W�F�N�g�ɓ��������ꍇ �܂���
        //�I�v�V�����̃`�F�b�N���X�g�ɖ߂�{�^���������ꂽ�ꍇ
        if (checkHitObj || checkPlayer)
        {
            if (checkHitPlayer)
            {
                //�v���C���[�̏d�͂����ɖ߂�
                playerRb.gravityScale = 1;

                //�e�q�֌W����
                gameObject.transform.DetachChildren();
            }

            //���D��j��
            Destroy(gameObject);
        }

        //�����������ꍇ
        if (lifeTime > lifeSpan && !checkHitPlayer)
        {
            Destroy(gameObject);
        }
    }

    //�v���C���[�ƃI�u�W�F�N�g�Ƃ̓����蔻��𒲂ׂ�
    void CheckPlayersCollider()
    {
        if (playerController != null)
        {
            isHittingCollider = playerController.IsHittingCollider;

            if (isHittingCollider)
            {
                checkPlayer = true;
            }
        }
    }
}
