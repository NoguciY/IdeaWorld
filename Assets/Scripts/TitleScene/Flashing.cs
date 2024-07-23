using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flashing : MonoBehaviour
{
    [Header("�_�ł��������C���[�W�R���|�[�l���g"), SerializeField]
    Image image;

    [Header("�ω����������F�̐ݒ�"), SerializeField]
    Color changeColor;

    [Header("�ݒ�̐F�ɕς��܂ł̎���"), SerializeField]
    float changeTime;
    [Header("���̐F�ɖ߂�܂ł̎���"), SerializeField]
    float returnTime;

    //���̐F�����Ă���
    Color originalColor;

    //����
    float time;

    //����
    float ratio;

    // �ω������ǂ����̃t���O
    bool isChanging = true;




    private void Start()
    {
        //���̐F����
        originalColor = image.color;
    }
    void Update()
    {
        //���Ԃ��v��
        time += Time.deltaTime;

        if (isChanging)
        {
            //����/�ύX�܂ł̎��ԁ@�@�̊������o��
            ratio = time / changeTime;

            //�@ratio��(0�`�P)�͈̔͂ɒ���
            ratio = Mathf.Clamp01(ratio);

            //�������g���ĐF��ύX
            image.color = originalColor + (changeColor - originalColor) * ratio;
        }
        else
        {
            //����/�ύX�܂ł̎��ԁ@�@�̊������o��
            ratio = time / returnTime;

            //�@ratio��(0�`�P)�͈̔͂ɒ���
            ratio = Mathf.Clamp01(ratio);

            //�������g���ĐF��ύX
            image.color = changeColor + (originalColor-changeColor) * ratio;

        }

        if (ratio >= 1)
        {
            //�F��ς���̂𔽓]
            isChanging = !isChanging;

            //���Ԃ����Z�b�g
            time = 0;
        }


    }
}
