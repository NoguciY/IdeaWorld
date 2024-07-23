using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionButtons : MonoBehaviour
{
    public GameObject titleButton;
    public GameObject returnButton;
    public GameObject tutorialButton;
    public GameObject checkPointButton;
    public GameObject optionButtonsPanel;

    public GameObject tutorialImage;

    //�I�v�V�����{�^�����������̏���
    public void Option_Button()
    {
        //���Ԃ��~�߂�
        Time.timeScale = 0;
        Debug.Log("�~�܂���");


        //�I�v�V�����{�^���͔�\��
        gameObject.SetActive(false);


        //�F�X�ȃ{�^����\������
        optionButtonsPanel.SetActive(true);
       
    }



    //�`���[�g���A����ʕ\������
    public void TutorialDisplay()
    {
        tutorialImage.SetActive(true);

        optionButtonsPanel.SetActive(false);
    }



    //�Q�[���ɖ߂�
    public void return_Button()
    {
        Time.timeScale = 1;
        Debug.Log("���͓����o��...");

        gameObject.SetActive(true);

        optionButtonsPanel.SetActive(false);
    }
    
    public void Title_Button()
    {
        Cursor.visible = true;
        Time.timeScale = 1;
        //optionButtonsPanel.SetActive(true);
        gameObject.SetActive(true);
        SceneManager.LoadScene("TitleScene");
    }

    // �I�v�V�����{�^������ʂɕ\���̐؂�ւ�
    public void OpsionButtonsPanelSwitching(bool s)
    {
        optionButtonsPanel.SetActive(s);
    }
}
