using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

//�Ŕɓ������s�L�[�����摜���\���As�L�[�������ƃq���g���o��
//������x�����ƃq���g�����
//�摜����\���ɂȂ����u�Ԃɉ��̏����ŕ\������邽��
//�L�[����͂�����ɂ��̃L�[�ɑ΂��ď������������𔻕ʂ���ϐ���p�ӂ���

public class TipsText : MonoBehaviour
{
    public AudioClip sound1;

    [SerializeField] GameObject hintImage;                  //�q���g�摜
    [SerializeField] GameObject hintKeyImage;               //�q���g�L�[�摜
    [SerializeField] PlayerController playerController;     //�v���C���[���L�[���͂��������m�F����
    [SerializeField] RectTransform canvasRectTrans;         //�q���g�����摜�̐e�̃L�����o�X
    [SerializeField] Camera targetCamera;                   //�I�u�W�F�N�g���f���J����

    AudioSource audioSource;                                //SE
    bool isHittingHintBoard = false;                        //�Ŕɓ������Ă��邩
    bool isPressedHintKey = false;                          //�q���g�L�[�������ꂽ��
    bool isDisplayingHint = false;                          //�q���g���\������Ă��邩
    bool isPerformedHintKeyInputProcessing = false;         //�q���g�L�[���͏������s������
    int hintImageAlfaValue = 0;                             //�q���g�摜�̃��l
    int hintKeyImageAlfaValue = 0;                          //�q���g�L�[�摜�̃��l

    const int maxAlfaValue = 255;                           //�ő僿�l
    const int minAlfaValue = 0;                             //�ŏ����l

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //�q���g�L�[����̍X�V
        isPressedHintKey = playerController.IsPressedHintKey;

        //�L�[���������ꂽ ���� �q���g���\������Ă���ꍇ
        if (isPressedHintKey && isDisplayingHint)
        {
            //�L�[���͏������s����
            isPerformedHintKeyInputProcessing = true;

            //�q���g���\��
            isDisplayingHint = false;
            hintImageAlfaValue = minAlfaValue;
            hintImage.GetComponent<Image>().color = new Color(255, 255, 255, hintImageAlfaValue);
        }

        //�Ŕɓ������Ă��� ���� �L�[�������ꂽ ���� �q���g���\������Ă��Ȃ��ꍇ
        if (isHittingHintBoard && isPressedHintKey && !isDisplayingHint)
        {
            //�L�[���͏��������̃t���[���ōs���Ă��Ȃ��ꍇ
            if (!isPerformedHintKeyInputProcessing)
            {
                //�q���g�L�[�����摜���\��
                hintKeyImageAlfaValue = minAlfaValue;
                hintKeyImage.GetComponent<Image>().color = new Color(255, 255, 255, hintKeyImageAlfaValue);

                //SE�Đ�
                AudioSource.PlayClipAtPoint(sound1, transform.position, 1f);

                //�q���g��\��
                isDisplayingHint = true;
                hintImageAlfaValue = maxAlfaValue;
                hintImage.GetComponent<Image>().color = new Color(255, 255, 255, hintImageAlfaValue);
            }
            else { isPerformedHintKeyInputProcessing = false; }
            }
    }

    //�v���C���[���q���g�|�C���g�ɓ��������u�ԁA�q���g��\��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHittingHintBoard = true;

            //�q���g�Ŕ̐^��Ƀq���g�����摜��\��������
            Vector2 hintBoardPos = (Vector2)transform.position + Vector2.up * 2.0f;     //�q���g�Ŕ̍��W
            var hintKeyImagePos = Vector2.zero;                                         //�q���g�����摜�̍��W

            //(���[���h�j���W���X�N���[�����W�ɕϊ�����
            hintBoardPos = targetCamera.WorldToScreenPoint(hintBoardPos);

            //�X�N���[�����W��RectTransform���W�ɕϊ�����
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, hintBoardPos, null, out hintKeyImagePos);

            hintKeyImage.GetComponent<RectTransform>().localPosition = hintKeyImagePos;

            //�q���g�L�[�����摜��\��
            hintKeyImageAlfaValue = maxAlfaValue;
            hintKeyImage.GetComponent<Image>().color = new Color(255, 255, 255, hintKeyImageAlfaValue);
        }
    }

    //�v���C���[���q���g�|�C���g���痣�ꂽ�u�ԁA�q���g���\��
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHittingHintBoard = false;
            
            //�q���g�L�[�摜���\������Ă���ꍇ
            if (hintKeyImageAlfaValue != minAlfaValue)
            {
                //�q���g�L�[�摜���\��
                hintKeyImageAlfaValue = minAlfaValue;
                hintKeyImage.GetComponent<Image>().color = new Color(255, 255, 255, hintKeyImageAlfaValue);
            }
        }
    }
}
