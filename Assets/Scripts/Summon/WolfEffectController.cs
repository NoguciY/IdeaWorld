using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�T�̖i����A�j���[�V�������ɕ\������摜�Ɋւ���N���X
//�����͉摜�̃��l���ŏ��ɂ���
//�i����Ƃ��Ƀ��l���ő�ɂ���
//�\��������A���X�ɓ����x�������Ĕ�\���ɂ���

public class WolfEffectController : MonoBehaviour
{
    [SerializeField] float displayedImageTime = 1.2f;   //�摜��\�������鎞��

    const int maxColorA = 255;                          //�ő僿�l
    const int minColorA = 0;                            //�Œჿ�l
    const float untilBarkWaitingTime = 0.5f;            //�i����܂ł̑҂�����
    bool isBarking = false;                             //�T���i������
    CircleCollider2D circleCollider;                    //�G�t�F�N�g���\�����Ă���ԗL��
    GameObject[] stageObjects = new GameObject[50];     //�G�t�F�N�g�ɓ��������I�u�W�F�N�g(�n�ʂƐ�)���i�[����
    int stageObjectCounter = 0;                         //OrderInLayer��������I�u�W�F�N�g�̐����J�E���g

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBarking)
        {
            isBarking = false;
            StartCoroutine(ControllImageAlfa());
        }
    }

    //���l��ϓ�������
    IEnumerator ControllImageAlfa()
    {
        yield return new WaitForSeconds(untilBarkWaitingTime);

        //�摜��\������
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, maxColorA);
        
        //�R���C�_�[��L���ɂ���
        circleCollider.enabled = true;

        yield return new WaitForSeconds(displayedImageTime);

        //�摜�𓧉߂�����
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, minColorA);
        
        //�G�t�F�N�g����������X�e�[�W�I�u�W�F�N�g��O�ɏo��
        for (int j = 0; j < stageObjectCounter; j++) 
        {
            stageObjects[j].GetComponent<SpriteRenderer>().sortingOrder = 65;
            stageObjects[j] = null;
        }

        stageObjectCounter = 0;

        //�R���C�_�[�𖳌��ɂ���
        circleCollider.enabled = false;
    }

    public void ActiveControllImageAFlag()
    {
        isBarking = true;
    }

    //�G�t�F�N�g��OrderInLayer��n�ʂƐ�̃I�u�W�F�N�g���O�ɂ���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")||
            collision.gameObject.CompareTag("River"))
        {
            stageObjects[stageObjectCounter] = collision.gameObject;
            stageObjects[stageObjectCounter].GetComponent<SpriteRenderer>().sortingOrder = 15;
            stageObjectCounter++;
        }
    }
}
