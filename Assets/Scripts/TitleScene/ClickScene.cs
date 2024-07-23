using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickScene : MonoBehaviour
{
    [Header("�N���b�N�o����悤�ɂȂ�܂ł̎��Ԑݒ�"), SerializeField]
    float time;

    //�J�E���g�_�E���p�ϐ�
    float t;

    [Header("�N���b�N�p�l��"), SerializeField]
    GameObject ClickPanel;

    [Header("�N���b�N��ɑJ�ڂ������V�[���̖��O"), SerializeField]
    string seneName;

    void Start()
    {
        //���Ԃ�ݒ�
        t = time;
    }

    void Update()
    {

        if(t>0)
        {
            //�J�E���g�_�E��
            t -= Time.deltaTime;
        }
        else if(t<0)
        {
            //��m�\��
            ClickPanel.SetActive(true);

            //�N���b�N�Ń^�C�g���V�[���ɑJ��
            if(Input.GetMouseButtonDown(0))
            {
                //�}�E�X�J�[�\����\��
                Cursor.visible = true;

                SceneManager.LoadScene(seneName);
            }
        }

    }
}
