using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public bool activeWolfSkill = false;    //�T���X�L����������

    [Tooltip("�}�̎Q��")]
    public GameObject branch;

    [Tooltip("���I�̍U���Ԋu")]
    public float interval;

    [Tooltip("�G�l�~�[�̈ړ����x")]
    public float speed;

    [SerializeField, Tooltip("�U���͈�(���a)")]
    private float range;

    [SerializeField, Tooltip("���G�͈�(���a)")] 
    float targetSearchRange;

    [SerializeField, Tooltip("���ˈʒu�������Ɋ��蓖�Ă�")]
    Transform firingPosition;

    [SerializeField, Tooltip("�s���͈͂̔������p")] 
    float offsetX = 1.0f;

    [SerializeField, Tooltip("�A�j���[�V�������ꎞ��~���鎞��")] 
    float animationPauseTime = 0.7f;

    /// <summary>
    /// �ˏo����I�u�W�F�N�g
    /// </summary>
    [SerializeField, Tooltip("�ˏo����I�u�W�F�N�g�������Ɋ��蓖�Ă�")]
    GameObject[] ThrowingObject;

    /// <summary>
    /// �W�I�̃I�u�W�F�N�g
    /// </summary>
    [SerializeField, Tooltip("�W�I�̃I�u�W�F�N�g�������Ɋ��蓖�Ă�")]
    GameObject TargetObject;

    /// <summary>
    /// �ˏo�p�x
    /// </summary>
    [SerializeField, Range(0F, 90F), Tooltip("�ˏo����p�x")]
    float ThrowingAngle;

    int index = 0;                          //ThrowingObject��index
    float timeCount;                        //���Ԍv��
    float time;                 
    float randamSpan;           
    float branchLeftEdgePosX;               //�}�̍��[�̈ʒu
    float branchRightEdgePosX;              //�}�̉E�[�̈ʒu
    float branchPosx;                       //�}�̒��S���W
    float branchScaleX;                     //�}�̉���
    Vector3 scale;                          //�v���C���[�̕��������Ƃ��ɁA�X�P�[�����]�Ŏg��
    Animator animator;                      //�A�j���[�V�����p
    bool canRestartAnimation;               //�A�j���[�V�������ĊJ�ł��邩
    string animationStateName = "Attack";   //�ꎞ��~������A�j���[�V������

    void Start()
    {
        //�U���̃C���^�[�o��
        timeCount = interval;

        //�X�P�[�����L��
        scale = transform.localScale;

        //���̈ړ������̂��߂Ɏ}�̉����ƒ��S���W���擾
        if (branch)
        {
            branchScaleX = branch.GetComponent<BoxCollider2D>().size.x;
        }
        branchPosx = branch.transform.position.x;

        branchLeftEdgePosX = branchPosx - (branchScaleX / 2) + offsetX;
        branchRightEdgePosX = branchPosx + (branchScaleX / 2) - offsetX;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeWolfSkill)
        {
            //�^�[�Q�b�g�����G�͈͓��̏ꍇ
            if (Mathf.Abs(transform.position.x - TargetObject.transform.position.x) <= targetSearchRange)
            {
                //�E�Ɉړ�
                if (transform.position.x + range < TargetObject.transform.position.x && branchRightEdgePosX > transform.position.x)
                {
                    transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
                }
                //���Ɉړ�
                else if (transform.position.x - range > TargetObject.transform.position.x && branchLeftEdgePosX < transform.position.x)
                {
                    transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
                }
            }

            if (Mathf.Abs(transform.position.x - TargetObject.transform.position.x) <= range)
            {
                timeCount -= Time.deltaTime;

                if (timeCount < 0)
                {
                    index = Random.Range(0, ThrowingObject.Length);
                    ThrowingBall(index);
                    timeCount = interval;
                }
            }

            //�^�[�Q�b�g�̕�������
            LookTarget();
        }

        //�U���A�j���[�V�����̊J�n���Ԃ𔻒f����
        canRestartAnimation = IsIndicateSpecifiedAnimationTime(animationStateName, animationPauseTime);
    }

    private void ThrowingBall(int index)
    {
        if (ThrowingObject != null && TargetObject != null)
        {
            //�U�����s��
            StartCoroutine(AttackCoroutine());
        }
        else
        {
            throw new System.Exception("�ˏo����I�u�W�F�N�g�܂��͕W�I�̃I�u�W�F�N�g�����ݒ�ł��B");
        }
    }

    /// <summary>
    /// �W�I�ɖ�������ˏo���x�̌v�Z
    /// </summary>
    /// <param name="pointA">�ˏo�J�n���W</param>
    /// <param name="pointB">�W�I�̍��W</param>
    /// <returns>�ˏo���x</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // �ˏo�p�����W�A���ɕϊ�
        float rad = angle * Mathf.PI / 180;

        // ���������̋���x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // ���������̋���y
        float y = pointA.y - pointB.y;

        // �Ε����˂̌����������x�ɂ��ĉ���
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // �����𖞂����������Z�o�ł��Ȃ����Vector3.zero��Ԃ�
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    //�v���C���[�Ƀ_���[�W��^����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();

            if (player != null)
            {
                player.PlayerDamage(1);
            }
        }
    }

    void LookTarget()
    {
        if (transform.position.x < TargetObject.transform.position.x)
        {
            transform.localScale = new Vector3(-scale.x, scale.y, 0);
        }
        else
        {
            transform.localScale = new Vector3(scale.x, scale.y, 0);
        }
    }

    //�U���A�j���[�V�����̌㐔�b�ԑҋ@�����čU��������
    IEnumerator AttackCoroutine()
    {
        //�A�j���[�V�������Đ�
        animator.SetTrigger("AttackTrigger");

        yield return new WaitForSeconds(1.2f);

        // Ball�I�u�W�F�N�g�̐���
        GameObject ball = Instantiate(ThrowingObject[index], firingPosition.transform.position, Quaternion.identity);

        Vector3 targetPosition;

        if (Random.Range(0, 2) == 0)
        {
            // �W�I�̍��W
            targetPosition = TargetObject.transform.position;
        }
        else
        {
            if (TargetObject.transform.localScale.x > 0)
            {
                targetPosition = TargetObject.transform.position + new Vector3(5, 0, 0);
            }
            else
            {
                targetPosition = TargetObject.transform.position - new Vector3(5, 0, 0);
            }
        }

        // �ˏo�p�x
        float angle = ThrowingAngle;

        // �ˏo���x���Z�o
        Vector3 velocity = CalculateVelocity(firingPosition.transform.position, targetPosition, angle);

        // �ˏo
        Rigidbody2D rid = ball.GetComponent<Rigidbody2D>();
        rid.AddForce(velocity * rid.mass, ForceMode2D.Impulse);
    }

    //�A�j���[�V�����̍Đ����Ԃ��J�E���g���Ďw�肵���^�C�~���O��m�点��֐�
    bool IsIndicateSpecifiedAnimationTime(string animationStateName, float specifiedTiming)
    {
        float animationTime;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
        {
            //�i����A�j���[�V�����̏����擾
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] animatorClip = animator.GetCurrentAnimatorClipInfo(0);
            //�i����A�j���̌��݂̍Đ����Ԃ��擾
            animationTime = animatorClip[0].clip.length * animationState.normalizedTime;
        }
        else
        {
            animationTime = 0;
        }

        //�A�j���[�V�����̌o�ߎ��Ԃ��w�肵�����Ԉȏ�̏ꍇ
        if (animationTime >= specifiedTiming)
        { return true; }
        else { return false; }
    }
}

