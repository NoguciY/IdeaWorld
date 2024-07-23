using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��(����)�̓����Ɋւ���N���X

//�v���C���[�܂��͘T���߂Â��ƕ\��ς��
//�����ƒʏ펞�ɖ߂�

//�T�̖i����U���œ������~�߂āA
//�����G�t�F�N�g���o��
//�����Ɉړ�
//���̌�A���b��ɏ�����

public class EnemiesController : MonoBehaviour
{
    [SerializeField] float runSpeed;                    //�����鑬�x
    [SerializeField] float extinctionTime;              //���Ŏ���
    [SerializeField] GameObject[] effectSprites;        //�G�̃G�t�F�N�g
    [SerializeField] private GameObject targetObject;   //�^�[�Q�b�g�̃Q�[���I�u�W�F�N�g
    [SerializeField] private float searchRange;         //�G�̍��G�͈�
    [SerializeField] float untilBarkWaitingTime = 0.8f; //�i����܂ł̑҂�����

    const int maxColorA = 255;                          //�摜�̍ő僿�l

    bool isBarkingFromWolf = false;                     //�T����i����ꂽ��
    bool activerRunAwayEnemies = false;                 //�G��ގU�����邩
    bool targetFinding = false;                         //�^�[�Q�b�g����������
    float deltaTime;                                    //�o�ߎ���

    public bool TargetFinding
    {
        get { return targetFinding; }
        private set { targetFinding = value; }
    }

    public bool IsBarkingFromWolf
    {
        get { return isBarkingFromWolf; }
        private set { isBarkingFromWolf = value; }
    }

    private void Update()
    {
        //�^�[�Q�b�g�����G�͈͓��̏ꍇ
        if (Mathf.Abs(transform.position.x - targetObject.transform.position.x) <= searchRange)
        {
            if (!targetFinding)
            { TargetFinding = true; }
        }

        //�T���X�L�����������ꍇ
        if (isBarkingFromWolf)
        {
            isBarkingFromWolf = false;
            //�G�t�F�N�g��\������
            StartCoroutine(DisplayEffect());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activerRunAwayEnemies)
        {
            RunAwayEnemies();
        }
    }
    
    //�T����i����ꂽ�ꍇ�t���O�I��
    public void ActiveBarkingFlag()
    {
        isBarkingFromWolf = true;
    }

    //�T����i����ꂽ�ꍇ�t���O�I��
    public void ActiveRunAwayEnemies()
    {
        activerRunAwayEnemies = true;
    }

    //����ގU������
    void RunAwayEnemies()
    {
        deltaTime += Time.deltaTime;
        transform.position += Vector3.right * runSpeed;

        if (deltaTime > extinctionTime)
        {
            Destroy(gameObject);
        }
    }

    //�G�t�F�N�g�̉摜��\������
    IEnumerator DisplayEffect()
    {
        yield return new WaitForSeconds(untilBarkWaitingTime);

        for (int i = 0; i < effectSprites.Length; i++)
        {
            effectSprites[i].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, maxColorA);
        }
    }
}
