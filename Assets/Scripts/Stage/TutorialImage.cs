using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class TutorialImage : MonoBehaviour
{ 
    [Header("�`���[�g���A���̉摜(0���珇�Ԃɓ���Ă�)"), SerializeField]
    Sprite[] tutorialImages;
    public int index=0;

    [Header("�ύX������Image�̎Q�Ƃ����Ă�"),SerializeField]
    Image tutorialImage;

    [SerializeField] GameObject optionButoon;


    private void Start()
    {
        //�z��ɓ�����Ă���Sprite�̂O�Ԗڂ�Image�ɓ����
        tutorialImage.sprite = tutorialImages[0];
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ImageInForeground();
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            NextImage();
        }

    }
    void NextImage()
    {
        index++;
        CheckIndex();
        tutorialImage.sprite = tutorialImages[index];
    }
    void ImageInForeground()
    {
        index--;
        CheckIndex();
        tutorialImage.sprite = tutorialImages[index];

    }
    void CheckIndex()
    {
        if (index < 0)
        {
            index = 0;
        }
        else if (index > tutorialImages.Length-1)
        {
            //�C���f�b�N�X�p�ϐ���0�ɖ߂��Ă���
            index = 0;

            //����
            TutorialClose();

        }
    }

    //�{�^���ŕ���Ƃ��p
    public void TutorialClose()
    {
        //�ŏ��̉摜�ɖ߂��Ă���
        tutorialImage.sprite = tutorialImages[0];

        //��\���ɂ���
        gameObject.SetActive(false);

        //Option�{�^���́A�\��
        optionButoon.SetActive(true);

        //���Ԃ�i�ނ悤�ɂ���
        Time.timeScale = 1;
    }


}
