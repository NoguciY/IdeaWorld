using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���C���J�������Ǘ�����N���X

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public GameObject subScreen;    //�w�i�̎�O�ɐݒu����T�u�w�i
    public float leftLimit = 0;     //���X�N���[���̏��
    public float rightLimit = 0;    //�E�X�N���[���̏��
    public float topLimit = 0;      //��X�N���[���̏��
    public float bottomLimit = 0;   //���X�N���[���̏��

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�����݂���ꍇ
        if (player != null)
        {
            //�J�����̍��W�X�V
            float x = player.transform.position.x;
            float y = player.transform.position.y + 3.0f;
            float z = transform.position.z;

            //������������
            //�㉺�Ɉړ�������t����
            if (x < leftLimit)
            {
                x = leftLimit;
            }
            else if (x > rightLimit)
            {
                x = rightLimit;
            }

            //�c����������
            //�㉺�Ɉړ�������t����
            if (y < bottomLimit)
            {
                y = bottomLimit;
            }
            else if (y > topLimit)
            {
                y = topLimit;
            }

            //�J�����ʒu��Vector3�����
            Vector3 v3 = new Vector3(x, y, z);
            transform.position = v3;

            //�T�u�X�N���[���X�N���[��
            if (subScreen != null)
            {
                y = subScreen.transform.position.y;
                z = subScreen.transform.position.z;
                Vector3 v = new Vector3(x / 2.0f, y, z);
                subScreen.transform.position = v;
            }
        }
    }
}
