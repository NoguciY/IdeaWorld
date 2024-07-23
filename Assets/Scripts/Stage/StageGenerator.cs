using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;        //�t�@�C������ɁuFile�v�N���X���g�p���ăt�@�C����ǂݍ��񂾂�A�������񂾂�A�f�[�^��񋓂��邽��
using UnityEditor;

public class StageGenerator : MonoBehaviour
{
    const float defaultStageObjWidth = 1.8f;
    const float defaultStageObjHeight = 1.0f;
    const float intiPosX = -10.7f;  //�����ʒu(x���W)
    const float intiPosY = -5.0f;   //�����ʒu(y���W)

    //CSV�̒��g�����郊�X�g(�񎟌��z��)
    List<string[]> csvDatas = new List<string[]>();
    GameObject stageObj;

    //csv�f�[�^�̓ǂݍ���
    public void ReadStageData()
    {
        //Resources�t�H���_����csv�ǂݍ���
        //TextAsset�^�̕ϐ���Resourcesn���̓ǂݍ���CSV�t�@�C�����i�[����
        //Unity�ŃX�N���v�g���Œ��ڂ��ǂݍ��݂��\�ɂȂ�
        TextAsset csvFile = Resources.Load("csv/IdeaWorld_Stage1") as TextAsset;
        
        //TextAsset�^��csvFile�ϐ���StringReader�ɕϊ�
        StringReader reader = new StringReader(csvFile.text);
        
        string ObjectAddress = "StageObject/";

        int maxX = -1;      //csv�t�@�C���̗񐔁A�����l�͈ꉞ-1
        int maxY = -1;      //csv�t�@�C���̍s���A�����l�́A�z��0����n�܂邽��-1
        string line = "";   //csv�t�@�C���P�s�����i�[����ϐ�

        //�u,�v�ŕ�������s���ǂݍ��݃��X�g�ɒǉ����Ă���
        //reader.Peaek��-1�ɂȂ�܂�
        //csv�t�@�C���̓��e���P�s���ǂݍ���
        //reader.Peek():���̕��������݂��邩�ǂ����𔻒f����
        while (reader.Peek() != -1)
        {
            //�ǂݍ���csv�t�@�C�����P�s���ǂݍ���line�ϐ��Ɋi�[
            line = reader.ReadLine();

            //string.Split(''):�������()�̕����ŕ�������
            //csvDatas���u,�v���Ƃɋ�؂�A��؂����f�[�^(0��1)�����X�g�ɒǉ�
            csvDatas.Add(line.Split(','));

            //�s�����J�E���g
            maxY++;
        }
        //csv�t�@�C���P�s���Ƃ̗񐔂��i�[
        maxX = CountChar(line, ',');

        //�����܂ł�csv�t�@�C���̍s���Ɨ񐔂��擾���Ă���

        //�񐔕��J��Ԃ�
        for (int x = 0; x <= maxX; ++x)
        {
            //�s�����J��Ԃ�
            for (int y = 0; y <= maxY; ++y)
            {
                //csv�̃f�[�^��0�łȂ��ꍇ
                if (csvDatas[y][x] != "0")
                {
                    //�X�e�[�W�̃I�u�W�F�N�g��z�u����
                    //�I�u�W�F�N�g�̈ʒu(���݂̗�,�S�s�� - ���݂̍s��)
                    //��������I�u�W�F�N�g�Ɣz�u����(�n�ʂ̈ʒu�𒲐����₷������)
                    CreateStageObject(maxY - y, x, ObjectAddress + csvDatas[y][x]);
                }
            }
        }
    }

    //�v���n�u���쐬����
    //�������F��������y���W
    //�������F��������x���W
    //��O�����F��������I�u�W�F�N�g�̖��O
    private void CreateStageObject(int y, int x, string objName)
    {
        //��������X�e�[�W�̃I�u�W�F�N�g���擾
        GameObject stageObjPrefab = (GameObject)Resources.Load(objName);

        //�I�u�W�F�N�g�̏c���X�P�[��
        float objSizeX = stageObjPrefab.transform.localScale.x;
        float objSizeY = stageObjPrefab.transform.localScale.y;

        //�I�u�W�F�N�g�𐶐�����ʒu = �X�e�[�W1�}�X�̍���
        Vector2 stageObjPos = new Vector2(x * defaultStageObjWidth + intiPosX,
                                           y * defaultStageObjHeight + intiPosY);

        //�I�u�W�F�N�g�𐶐�����
        stageObj = Instantiate(stageObjPrefab, stageObjPos, Quaternion.identity);

        //�X�e�[�W1�}�X�̍������ɂ��ăI�u�W�F�N�g�̈ʒu�𐮂���
        stageObj.transform.position += new Vector3(defaultStageObjWidth / 2 * objSizeX, -(defaultStageObjHeight / 2.0f * objSizeY), 0);

        //�I�u�W�F�N�g�̒��ɓ����
        stageObj.transform.parent = transform;
    }

    //�񐔂��J�E���g����
    public static int CountChar(string s, char c)
    {
        //string.Replace("�u����������������","�u�������镶����") : ������̈ꕔ��ʂ̕�����ɒu��������
        //string.ToString : int�^��flaot�^��string�^�ɕϊ�����
        //                ��char�^��string�^�ɕϊ��ł��遚
        //csv�t�@�C���̂P�s���̕�������
        //csv�t�@�C���̂P�s���́u,�v�𖳂Ɠ���ւ����������ň�������������Ԃ�
        //�u,�v�̐����Ԃ����(��(�z��)��0����J�E���g����)
        return s.Length - s.Replace(c.ToString(), "").Length;
    }

    void Awake()
    {
        //�X�e�[�W�̃f�[�^��ǂݍ��݁A�X�e�[�W���쐬����
        ReadStageData();
    }
}
