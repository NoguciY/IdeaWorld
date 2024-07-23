using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�T�̖i����A�j���[�V�������ɕ\������摜�Ɋւ���N���X
//�����͉摜�̃��l���ŏ��ɂ���
//�i����Ƃ��Ƀ��l���ő�ɂ���
//�\��������A���X�ɓ����x�������Ĕ�\���ɂ���

public class EnemyEffectController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] float untilBarkWaitingTime = 0.7f; //�i����܂ł̑҂�����
    [SerializeField] float displayedImageTime = 3.0f;   //�摜��\�������鎞��

    const int maxColorA = 255;                          //�ő僿�l
    const int minColorA = 0;                            //�Œჿ�l
    bool isBarkingWolf = false;                         //�T���i�������ǂ���

    void Update()
    {
        //�T���X�L�����������ꍇ�A�G�t�F�N�g��\������
        if (enemyController.activeWolfSkill && !isBarkingWolf)
        {
            isBarkingWolf = true;
            StartCoroutine(ControllImageAlfa());
        }

        //�T���X�L���������ĂȂ��ꍇ�A�i���Ă��Ȃ�
        if (!enemyController.activeWolfSkill && isBarkingWolf)
        {
            isBarkingWolf = false;
        }
    }
    
    //���l��ϓ�������
    IEnumerator ControllImageAlfa()
    {
        yield return new WaitForSeconds(untilBarkWaitingTime);

        //�摜��\������
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, maxColorA);

        yield return new WaitForSeconds(displayedImageTime);

        //�摜�𓧉߂�����
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, minColorA);
    }
}
