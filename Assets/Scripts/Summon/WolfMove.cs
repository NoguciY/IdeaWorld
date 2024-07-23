using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//�T�Ɋւ���N���X

public class WolfMove : MonoBehaviour
{
    public float keeppFromPlayerDistance = 2.0f;            //�T���v���C���[�ƕۂ���
    public float wolfSpeed = 2.0f;                          //�P�t���[���̈ړ���
    public float rayLength = 1.5f;                          //Ray�̒���
    public Vector2 maxJumpVec = new Vector2(2.0f, 6.0f);    //��W�����v�̃x�N�g��
    public LayerMask objectLayer;                           //���̃��C���[�̃I�u�W�F�N�g�̃^�O�ŃW�����v���邩��������

    GameObject player;          //�v���C���[
    WolfSkill wolfSkill;        //�G���������狳���Ă��炤����
    Rigidbody2D rigidb;         //�W�����v���鎞�g��
    Animator animator;
    float scaleX;               //���X�P�[��
    float toPlayerDistance;     //�v���C���[�Ƃ̋���
    Vector2 playerPos;          //�v���C���[�̍��W
    Vector2 wolfPos;            //�T(���g)�̍��W
    bool canJump = false;       //�W�����v�ł��邩
    bool onGround = false;      //�n��ɂ��邩
    bool wolfDirection;         //true���E����
    bool jumping = false;       //�W�����v�����ǂ���
    bool onLine = false;        //���C����ɂ��邩
    bool canWait;               //�ҋ@��Ԃɂł��邩

    //�T�̏��
    enum WolfState
    {
        Wait,     //�ҋ@
        Run,      //����
        Jump      //�W�����v
    }

    WolfState wolfState = WolfState.Wait;   //�n�߂͑ҋ@���

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //�v���C���[�I�u�W�F�N�g���擾
        player = GameObject.FindWithTag("Player");
        //�G���Ď����邽�߂̃X�N���v�g
        wolfSkill = GetComponent<WolfSkill>();
        //�v���C���[���U������Ƃ��Ɏg��
        scaleX = transform.localScale.x;

        rigidb = GetComponent<Rigidbody2D>();

        //�T�̌��������߂�
        TurnWolfDirection();

        //�ҋ@��Ԃɂ���
        ChangeState(WolfState.Wait);
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�����āA�G�����Ȃ��ꍇ
        if (player && !wolfSkill.enemy)
        {
            //�v���C���[�ƘT�̍��W���擾
            playerPos = player.transform.position;
            wolfPos = transform.position;

            //�T�ƃv���C���[�Ƃ̋���
            toPlayerDistance = playerPos.x - wolfPos.x;

            //�T�̌��������߂�
            TurnWolfDirection();

            //�n�㔻��
            CheckOnGround();

            //���n�_�̒n�㔻��
            CheckCanLandInGround();

            //��Ԃ��X�V����
            StateUpdate();

            //��Ԃɂ���ĘT���X�V����
            switch (wolfState)
            {
                case WolfState.Wait:

                    if (animator.GetBool("isRunning"))
                    {
                        //����A�j����~
                        animator.SetBool("isRunning", false);
                    }
                    //Debug.Log("WAIT");
                    break;

                case WolfState.Run:
                    //�v���C���[�Ƌ�����ۂ悤�ɓ���
                    KeepDistance();

                    if (!animator.GetBool("isRunning"))
                    {
                        //����A�j���Đ�
                        animator.SetBool("isRunning", true);
                    }
                    //Debug.Log("RUN");
                    break;

                case WolfState.Jump:
                    if (animator.GetBool("isRunning"))
                    {
                        //����A�j����~
                        animator.SetBool("isRunning", false);
                    }
                    //�W�����v����
                    Jump();

                    //Debug.Log("JUMP");
                    break;

                default:
                    break;
            }
        }
    }

    //��Ԃ�ς���֐�
    void ChangeState(WolfState nextState) => wolfState = nextState;

    //�T�̐i�s�����𒲂ׂ�֐�
    //true  :�E����(��)�ɐi��
    //false :������(��)�ɐi��
    void TurnWolfDirection()
    {
        //�v���C���[���E�������Ă���ꍇ
        if (player.transform.localScale.x > 0)
        {
            //�E������
            wolfDirection = true;
            transform.localScale = new Vector2(scaleX * 1, transform.localScale.y);
        }
        //�v���C���[�����������Ă���ꍇ
        else
        {
            //��������
            wolfDirection = false;
            transform.localScale = new Vector2(scaleX * -1, transform.localScale.y);
        }
    }

    //�n��ɂ��邩�̔���
    void CheckOnGround()
    {
        Ray2D wolfFrontRay;         //�T�̑O�ɐ݂������
        Vector2 startRayPos;        //�����̎n�_
        if (wolfDirection)
        {
            //�����̎n�_ = �T�̕@��ӂ� (�E����)
            startRayPos = transform.position + new Vector3(0.7f, 0);
        }
        else
        {
            //�����̎n�_ = �T�̕@��ӂ� (������)
            startRayPos = transform.position + new Vector3(0.7f, 0) * -1.0f;
        }

        //�������擾
        wolfFrontRay = new Ray2D(startRayPos, Vector2.down);

        //Ray�ɓ��������I�u�W�F�N�g���C���[�̏����i�[
        //Physics2D.Raycast(�n�_, ����, ray�̒���, �Ώۃ��C���[)�֐�
        //�Ԃ�l:RaycastHit2D
        RaycastHit2D hit = Physics2D.Raycast(wolfFrontRay.origin,
                  wolfFrontRay.direction, rayLength, objectLayer);

        if (hit.collider)
        {
            //Ray�ɒn�ʂ��������Ă���ꍇ
            if (hit.collider.tag == "Ground")
            {
                //�n��
                onGround = true;
                onLine = false;
            }
            else if (hit.collider.tag == "Line")
            {
                onLine = true;
                onGround = false;
            }
            //Ray�ɒn�ʂ��������Ă��Ȃ��ꍇ
            else { onGround = false; onLine = false; }
        }
        else { onGround = false; onLine = false; }
    }

    //�W�����v���Ē��n�ł��邩�𒲂ׂ�֐�
    void CheckCanLandInGround()
    {
        Ray2D fromLandingPointRay;  //���n�_�𒲂ׂ����
        Vector2 landingPoint;       //�W�����v�̒��n�_
        //�W�����v�̔򋗗�
        float jumpingDistance = maxJumpVec.x * (maxJumpVec.y / (-Physics2D.gravity.y * 0.5f));

        if (wolfDirection)
        {
            //�����̎n�_ = �W�����v�̒��n�_(�E����)
            landingPoint = (Vector2)transform.position + new Vector2(jumpingDistance + 0.7f, 0);
        }
        else
        {
            //�����̎n�_ = �W�����v�̒��n�_(������)
            landingPoint = (Vector2)transform.position + new Vector2(jumpingDistance + 0.7f, 0) * -1.0f;
        }

        //�W�����v�̒��n�_���n�_�Ƃ���������擾
        fromLandingPointRay = new Ray2D(landingPoint, Vector2.down);

        //Ray�ɓ��������I�u�W�F�N�g���C���[�̃I�u�W�F�N�g�����i�[
        RaycastHit2D groundHit = Physics2D.Raycast(fromLandingPointRay.origin,
                     fromLandingPointRay.direction, rayLength, objectLayer);

        if (groundHit.collider)
        {
            //Ray���n�ʂɓ��������ꍇ�A�W�����v�\
            if (groundHit.collider.tag == "Ground")
            {
                canJump = true;
            }
            //Ray�����C���ɓ��������ꍇ�A�W�����v�\
            else if (groundHit.collider.tag == "Line")
            {
                canJump = true;
            }
            //�W�����v�s��
            else { canJump = false; }
        }
        else { canJump = false; }
    }

    //�����ԂŎ��s����֐�
    //�G��������܂ł̓v���C���[�Ƌ�����ۂ��ړ�
    void KeepDistance()
    {
        Vector2 moveVec = transform.position;

        //�T���E�����̏ꍇ
        if (wolfDirection)
        {
            //�E�Ɉړ�
            moveVec.x += wolfSpeed * Time.deltaTime;
        }
        //�T���������̏ꍇ
        else
        {
            moveVec.x -= wolfSpeed * Time.deltaTime;
        }
        transform.position = moveVec;
    }

    //�W�����v��ԂŎ��s����֐�
    //�W�����v���Ă���Ԃ́A�v���C���[�̉e���œ����Ȃ�
    void Jump()
    {
        //�W�����v���Ă��Ȃ�
        if (!jumping)
        {
            if (wolfDirection)
            {
                //�E�ɑ�W�����v
                rigidb.AddForce(maxJumpVec, ForceMode2D.Impulse);
            }
            else
            {
                //���ɑ�W�����v
                rigidb.AddForce(maxJumpVec * new Vector2(-1, 1), ForceMode2D.Impulse);
            }

            //�W�����v�A�j���Đ�
            animator.SetTrigger("JumpTrigger");
            //�W�����v��
            animator.SetBool("isJumping", true);
            Debug.Log("Jump");
            //�W�����v��ԂŃW�����v������Ȃ��A�W�����v������
            jumping = true;
        }
    }

    //��Ԃ𑖂��Ԃɕς���֐�
    bool ChangeRunState()
    {
        //Ray���n�ʂ����ɓ������Ă���ꍇ
        if (onGround || onLine)
        {
            //�E�����̏ꍇ
            if (wolfDirection)
            {
                //�v���C���[�ƘT���ۂ��W���T�̍��W���������ꍇ
                if (playerPos.x + keeppFromPlayerDistance > wolfPos.x)
                {
                    return true;
                }
            }
            //�������̏ꍇ
            else
            {
                //�v���C���[�ƘT�̋�����-2m���傫���ꍇ
                if (playerPos.x - keeppFromPlayerDistance < wolfPos.x)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //��Ԃ�ҋ@��Ԃɕς���֐�
    bool ChangeWaitState()
    {
        //Ray���n�ʂɓ������Ă��Ȃ��ꍇ ����
        //�ڂ̑O�ɔ�щz�����Ȃ������ǂ�����ꍇ
        if (!onGround && !canJump)
        { return true; }
        //�T���v���C���[��2m�O�ɂ���ꍇ
        else { return true; }
    }

    //��Ԃ��W�����v��Ԃɕς���֐�
    bool ChangeJumpState()
    {
        if (!jumping)
        {
            //�n��ɂ��ĂȂ����W�����v�\
            if (!onGround && canJump)
            {
                return true;
            }
        }
        return false;
    }

    //��Ԃ��X�V����֐�
    void StateUpdate()
    {
        if (ChangeJumpState())
        {
            //�W�����v��ԂɈڍs
            ChangeState(WolfState.Jump);
        }
        else if (ChangeRunState())
        {
            //�����ԂɈڍs
            ChangeState(WolfState.Run);
        }
        else if (ChangeWaitState())
        {
            //�ҋ@��ԂɈڍs
            ChangeState(WolfState.Wait);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�n�ʂɓ��������u��
        if (collision.gameObject.tag == "Ground")
        {
            //�W�����v���Ă��Ȃ�
            jumping = false;
        }

        //�W�����v�A�j�����璅�n�A�j���֑J��
        if (animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", false);
        }
    }
}