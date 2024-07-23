using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;              //��ԑJ�ڂ̊Ǘ�������N���X

    [SerializeField] private LayerMask objectLayer;     //���n�ł��郌�C���[

    [Header("�ړ����x")]
    [SerializeField] float moveSpeed;                   //�ړ����x

    [Header("�ő�ړ����x")]
    [SerializeField] float maxMoveSpeed;                //���x�𐧌�����

    [Header("�W�����v��")]
    [SerializeField] float jumpForce;                   //�W�����v��

    [Header("�ő�W�����v��")]
    [SerializeField] float maxJumpForce;                //�W�����v�͂𐧌�����

    [SerializeField] AudioSource runningAudioSource;    //����
    
    [SerializeField] AudioSource jumpingAudioSource;    //�W�����v��

    [SerializeField] PlayerManager playerManager;       //�v���C���[�Ǘ��p

    Rigidbody2D rigidb;                                 //�������Z�p
    bool onGround = false;                              //�n�ʂ̏�ɂ��邩
    float originalScaleX;                               //�v���C���[�̌��X�̉��X�P�[��
    float scaleX;                                       //���X�P�[��
    float axisH;                                        //���������̓���
    float runSpeed;                                     //���鑬�x
    float jumpSpeed;                                    //�W�����v�̑��x
    bool jumpKey;                                       //�W�����v�L�[�������ꂽ��
    bool isPressedHintKey;                              //�q���g�L�[�������ꂽ��
    bool isHittingCollider;                             //�R���C�_�[�ɓ������Ă��邩
    public bool playerMove = true;                      //�v���C���[������\��
    public bool knockback = false;                      //�m�b�N�o�b�N���邩
    BalloonController balloonController = null;         //���D�A�N�Z�X�p
    CapsuleCollider2D capsuleCollider;                  //�����蔻��
    PhysicsMaterial2D originalMaterial;                 //���C�͂�ύX���邽��

    //�A�N�Z�T
    public bool OnGround => onGround;
    public bool JumpKey => jumpKey;
    public bool IsPressedHintKey => isPressedHintKey;
    public bool IsHittingCollider => isHittingCollider;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float AxisH => axisH;
    public float OriginalScaleX => originalScaleX;
    public BalloonController BalloonController => balloonController;
    public Rigidbody2D Rigidb => rigidb;
    public AudioSource RunningAudioSource => runningAudioSource;
    public AudioSource JumpingAudioSource => jumpingAudioSource;

    public float RunSpeed
    {
        get { return runSpeed; }
        set { runSpeed = value; }
    }

    //�W�����v����
    public bool IsJumping { get; set; } = false;
    //���D�ɓ���������
    public bool HitBalloon { get; set; } = false;
    //�Ԃ牺���蒆��
    public bool IsHanging { get; set; } = false;

    public Animator Animator { get; private set; }

    private void Awake()
    {
        playerStateMachine = new PlayerStateMachine();
        playerStateMachine.Init(this, State.Idle);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        scaleX = transform.localScale.x;

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalMaterial = capsuleCollider.sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        //�ڒn����
        CheckOnGround();

        //�Ԃ牺�����ԂłȂ��Ƃ���isHittingCollider��false�ɂ���
        if (!IsHanging && isHittingCollider)
        {
            //�����������Ă��Ȃ�
            isHittingCollider = false;
        }

        //�`���Ă�Ƃ��͓����Ȃ�
        if (playerMove)
        {
            //�W�����v�L�[�̓��͂����m����
            jumpKey = Input.GetButtonDown("Jump");

            //a,d,��,���L�[�̓��͂����m����
            axisH = Input.GetAxis("Horizontal");

            //�q���g�\���{�^���̓��͂����m����
            isPressedHintKey = Input.GetKeyDown(KeyCode.S);

            //�Ԃ牺���蒆�ȊO�̓v���C���[���U������悤�ɂ���
            if (!balloonController)
            {
                if (axisH > 0)
                {
                    //�E������
                    transform.localScale = new Vector2(scaleX, transform.localScale.y);
                }
                else if (axisH < 0)
                {
                    //��������
                    transform.localScale = new Vector2(-scaleX, transform.localScale.y);
                }
            }
        }

        //���݂̏�Ԃ��X�V����
        playerStateMachine.Update();
    }

    private void FixedUpdate()
    {

        if (playerMove)
        {
            //�m�b�N�o�b�N�̏���
            if (knockback)
            {
                StartCoroutine(KnockBack());
                return;
            }
            //�m�b�N�o�b�N���Ă��Ȃ��ꍇ
            else
            {
                //�_�b�V���ƃW�����v������
                rigidb.velocity += new Vector2(runSpeed, jumpSpeed);

                //�ړ����x�𐧌�����
                if (rigidb.velocity.x > maxMoveSpeed)
                {
                    rigidb.velocity = new Vector2(maxMoveSpeed, rigidb.velocity.y);
                }
                else if (rigidb.velocity.x < -maxMoveSpeed)
                {
                    rigidb.velocity = new Vector2(-maxMoveSpeed, rigidb.velocity.y);
                }

                //�W�����v�͂𐧌�����
                if (rigidb.velocity.y > maxJumpForce)
                {
                    rigidb.velocity = new Vector2(rigidb.velocity.x, maxJumpForce);
                }
            }
        }
        //����`���Ă���Ƃ��̏���
        else
        {
            //���ړ��ł��Ȃ�����
            rigidb.velocity = Vector2.up * rigidb.velocity;
        }

        //�n�㎞�͖��C�͂�����ɂ���
        //���̂ق��ł͖��C�͂��Ȃ���
        if (onGround)
        {
            // �ꎞ�I�ȃR�s�[���쐬���ĕύX
            PhysicsMaterial2D temporaryMaterial = new PhysicsMaterial2D();
            temporaryMaterial.friction = 0.4f;

            // �ꎞ�I�ȃR�s�[��Collider�ɐݒ�
            capsuleCollider.sharedMaterial = temporaryMaterial;
        }
        else
        {
            // �I���W�i���̕����}�e���A���ɖ߂�
            capsuleCollider.sharedMaterial = originalMaterial;
        }
    }

    //�n�㔻�������֐�
    void CheckOnGround()
    {
        //�v���C���[�̌��_�����Y���̋���
        float underCheckOffsetY = 0.2f;

        //�~��Ray�̌��_
        Vector2 underCheckOrigin = (Vector2)transform.position + (Vector2.up * underCheckOffsetY);

        //�~��Ray�̔��a
        float underCheckRadius = 0.22f;

        //�~��Ray�̋���
        float underCheckDistance = 0.0f;

        //Ray�ɓ��������I�u�W�F�N�g�����i�[����
        RaycastHit2D underHit = Physics2D.CircleCast(underCheckOrigin, underCheckRadius,
                                                     Vector2.down, underCheckDistance, objectLayer);

        //Ray�Ɏw�肵�����C���[�����������ꍇ
        if (underHit.collider)
        {
            //�n�ʂ����������ꍇ
            if (underHit.collider.gameObject.tag == "Ground")
            {
                //�n��ɂ���
                onGround = true;
            }
            //���C�������������ꍇ
            if (underHit.collider.gameObject.tag == "Line")
            {
                //�n��ɂ���
                onGround = true;
            }
        }
        //�n��ɂ��Ȃ�
        else { onGround = false; }
    }

    //�J�v�Z�����C�f�o�b�O�p
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(
            transform.position + (Vector3.up * 0.2f),
            0.22f);
    }

    IEnumerator KnockBack()
    {
        //0.5�b��A�m�b�N�o�b�N�t���O�I�t
        yield return new WaitForSeconds(0.5f);
        knockback = false;
    }

    public bool OnDestroyBalloonFlag(Vector2 checkPoint)
    {
        //���D���ƃ`�F�b�N�|�C���g�ɖ߂�
        balloonController.transform.position = checkPoint;

        //���D���󂷂��߂̃t���O�I��
        isHittingCollider = true;
        return isHittingCollider;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsHanging)
        {
            //�n�ʁA�V��A���A�G�A�����I�u�W�F�N�g�ɓ��������ꍇ
            if (collision.gameObject.CompareTag("Ground")       ||
                collision.gameObject.CompareTag("Ceiling")      ||
                collision.gameObject.CompareTag("LineParent")   ||
                collision.gameObject.CompareTag("Enemy")        ||
                collision.gameObject.CompareTag("Enemies")      ||
                collision.gameObject.CompareTag("DeadObject"))
            {
                //���D���󂷂��߂̃t���O�I��
                isHittingCollider = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���D�ɓ��������ꍇ
        if (collision.gameObject.CompareTag("Balloon"))
        {
            balloonController = collision.gameObject.GetComponent<BalloonController>();
            HitBalloon = true;
        }

        if (collision.gameObject.CompareTag("DeadObject"))
        {
            //���D���󂷂��߂̃t���O�I��
            isHittingCollider = true;

            //�v���C���[�̃��C�t��0�ɂ���
            playerManager.PlayerDamage(3);
        }
    }

    //�e��ԂɈڍs������֐�
    public void Idle() => playerStateMachine.ChangeState(State.Idle);
    public void Run() => playerStateMachine.ChangeState(State.Run);
    public void Jump() => playerStateMachine.ChangeState(State.Jump);
    public void Hang() => playerStateMachine.ChangeState(State.Hang);
}
