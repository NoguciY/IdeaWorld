using UnityEngine;
using UnityEngine.UI;

//���C�tUI���v���C���[��HP�ɉ����čX�V����֐�


public class LifeUIManager : MonoBehaviour
{
    public GameObject player;       //�v���C���[
    public GameObject[] playerLifeUI = new GameObject[3];   //���C�tUIx3
    Color[] playerLifeUIColor = new Color[3];               //���C�t�̉摜�̏��������邽��
    int playerLife;                 //�v���C���[�̃��C�t
    int preLife;                    //�O�̃��C�t��ۑ�����
    PlayerManager playerManager;    //���݂�HP���擾���邽��
    RectTransform myRectTransform;  //���C�tUI�p�l���̃X�N���[�����W���g��
    int spacePushCount = 0;         //�X�y�[�X�L�[����������

    // Start is called before the first frame update
    void Start()
    {
        playerManager = player.GetComponent<PlayerManager>();
        myRectTransform = GetComponent<RectTransform>();

        //���C�tUI3��image���i�[
        for (int i = 0; i < 3; i++)
        {
            playerLifeUIColor[i] = playerLifeUI[i].GetComponent<Image>().color;
        }

        preLife = playerManager.GetLife();

    }

    // Update is called once per frame
    void Update()
    {
        //UI���X�N���[�����W�̓������W�ɌŒ肷��
        myRectTransform.position = myRectTransform.position;

        //UI���X�V����
        LifeUIUpdate();

        //�X�P�b�`��ʕ\�����͓����x��������
        LowLifeAlphaDisplayedSketchScreen();
    }

    //UI�����݂̃��C�t�ɍ��킹�čX�V����֐�
    void LifeUIUpdate()
    {
        //�v���C���[�̌��݂̃��C�t���擾
        playerLife = playerManager.GetLife();

        //���C�t���ς�����ꍇ
        if (playerLife != preLife)
        {
            //���C�t��3�̏ꍇ
            if (playerLife == 3)
            {
                preLife = playerLife;

                for (int i = 0; i < 3; i++)
                {
                    //���C�tUI��3�Ƃ�����
                    playerLifeUIColor[i].a = 1.0f;
                    playerLifeUI[i].GetComponent<Image>().color = playerLifeUIColor[i];
                }
            }
            //���C�t��2�̏ꍇ
            else if (playerLife == 2)
            {
                preLife = playerLife;

                //�E�̃��C�tUI�𔼓����ɂ���
                playerLifeUIColor[0].a = 0.5f;
                playerLifeUI[0].GetComponent<Image>().color = playerLifeUIColor[0];
            }
            //���C�t��1�̏ꍇ
            else if (playerLife == 1)
            {
                preLife = playerLife;

                //�����̃��C�tUI�𔼓����ɂ���
                playerLifeUIColor[1].a = 0.5f;
                playerLifeUI[1].GetComponent<Image>().color = playerLifeUIColor[1];
            }
            //���C�t��0�̏ꍇ
            else if (playerLife == 0)
            {
                preLife = playerLife;

                //���̃��C�tUI�𔼓����ɂ���
                playerLifeUIColor[2].a = 0.5f;
                playerLifeUI[2].GetComponent<Image>().color = playerLifeUIColor[2];
            }
        }
    }

    //�X�P�b�`��ʂ��\������Ă���Ԃ͓����x��������֐�
    void LowLifeAlphaDisplayedSketchScreen()
    {
        //�X�y�[�X�L�[���������ꍇ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //�J�E���g���v���X
            spacePushCount++;

            if (spacePushCount == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    //���C�tUI��3�Ƃ������x��������
                    playerLifeUIColor[i].a -= 0f;
                    playerLifeUI[i].GetComponent<Image>().color = playerLifeUIColor[i];
                    Debug.Log(spacePushCount);
                }
            }
            else
            {
                //�J�E���g�����Z�b�g
                spacePushCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    //���C�tUI��3�Ƃ����̓����x�ɖ߂�
                    playerLifeUIColor[i].a += 0f;
                    playerLifeUI[i].GetComponent<Image>().color = playerLifeUIColor[i];
                    Debug.Log(spacePushCount);
                }
            }
        }
    }
}