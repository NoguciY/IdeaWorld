using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchManager : MonoBehaviour
{
    [Header("�ď����܂ł̃C���^�[�o���b��")]
    public float sketchInterval;

    //�������\�ȏꍇ��true (���̃X�N���v�g�́Aenabled��false�ɂȂ��Ă��܂�����AGameManager�Ŏ��Ԃ��v���Ă�)
    [HideInInspector] public bool summons = true;




    [Header("�X�P�b�`���[�h��Trail�̎Q��")]
    [SerializeField] TrailRenderer trail;

    [Header("UI�֘A�̎Q��")]
    [SerializeField] UIManager uIManager;

    [SerializeField] GameObject optionButton;

    [Header("�I�u�W�F�N�g�����֘A�̎Q��")]
    [SerializeField] SummonedObjGenerator generator;

    [Header("�Q�[���}�l�[�W���[")]
    [SerializeField] GameManager gameManager;

    [Header("�v���C���[�֘A�̎Q��")]
    [SerializeField] PlayerController playerController;

    //���̏ゾ��true�ɂȂ�
    bool onPaper;

    //�`�����Ԃ��̊m�F
    bool sketchMaker = false;

    //����{�{�^�������������A�`�F�b�N�|�C���g�̐����L�^
    [HideInInspector] public int checkPoints;
    [HideInInspector] public int summonerNumber;


    void Update()
    {

        //���̏ォ�m�F�@(OnTriggerEnter2D���g�p)
        if (onPaper)
        {
            //�E�N���b�N�Ő���`���n�߂�
            if (Input.GetMouseButtonDown(0))
            {
                SketchMarker(true);
            }

            //�E�N���b�N�𗣂��ƕ`���̂��I��
            if (Input.GetMouseButtonUp(0))
            {
                SketchMarker(false);
                uIManager.ExampleDisplay(uIManager.IndexLastExample);

            }
        }
        else
        {
            //�`���̂��I��
            SketchMarker(false);
        }

    }

     //�X�P�b�`�ŕ`���Ă�r���Ń��[�h�̐؂�ւ������������̏���
    private void OnDisable()
    {
        SketchMarker(false);
    }

    //�X�P�b�`���[�h�ł̃}�[�J�[�́@ ( ON / OFF )
    public void SketchMarker(bool switchMarker)
    {
        //�`�����Ԃ̐؂�ւ�
        sketchMaker = switchMarker;

        if (switchMarker)
        {
            trail.time = 10000f;
            trail.startWidth = 0.5f;
        }
        else
        {
            trail.time = 0f;
            trail.startWidth = 0f;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //���̏�Ȃ�X�P�b�`�����`����
        if (collision.CompareTag("Paper"))
        {
            onPaper = true;
        }

        //�}�E�X�������Ȃ���`�F�b�N�|�C���g�ʉ�
        if (collision.CompareTag("CheckPoint") && sketchMaker)
        {
            //�`�F�b�N�|�C���g�������Ă���
            collision.gameObject.SetActive(false);
            //�`�F�b�N�|�C���g�ʉ߂����������A���炵�Ă���
            checkPoints--;

            if (checkPoints == 0)
            {
                //����������A�X�P�b�`���[�h��ʂ��\���ɂ���
                uIManager.SketchModeSwitch(false);

                //���[�h�ؑ�
                gameManager.SwitchingMarkersAndSketches(true);
                
                //�A�j���[�V����������ɏ�������
                StartCoroutine(skillCoroutine());

                //�������o���Ȃ���Ԃɐ؂�ւ�
                summons = false;

                //�N���N���Ɩ{�̕\��
                uIManager.SwitchingBookAndKurukuruDisplay(true);
            }
        }
        else if (collision.CompareTag("OverflowCheck"))
        {
            //�͂ݏo���Ƃ�蒼��
            uIManager.ExampleDisplay(uIManager.IndexLastExample);
            SketchMarker(false);
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //���̊O�Ȃ�X�P�b�`���`���Ȃ�
        if (collision.CompareTag("Paper"))
        {
            onPaper = false;
        }
    }


    IEnumerator skillCoroutine()
    {
        float endAnimationTime = 1.5f;  //�A�j���[�V�������I��鎞��

        //�v���C���[�𓮂��Ȃ�����
        playerController.playerMove = false;

        //�A�j���[�V�����Đ�
        playerController.Animator.SetTrigger("skillTrigger");
        playerController.Animator.SetBool("isActivatedSkill", true);

        yield return new WaitForSeconds(endAnimationTime);

        //����
        generator.SummonObj(summonerNumber);
        //Debug.Log("����");

        //�v���C���[�𓮂���悤�ɂ���
        playerController.playerMove = true;

        //����A�j���[�V�������Đ��\�ɂ���
        playerController.Animator.SetBool("isActivatedSkill", false);

        //�I�v�V�����{�^����\������
        uIManager.SwitchingOptionButtonDisplay(true);

        //�X�P�b�`�A�C�R���A���C�t�A�t�����[�L�[�A�C�R���̕\��
        uIManager.UIDisplaySwitchingInSketchMode(true);
    }
}


