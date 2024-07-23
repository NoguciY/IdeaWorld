using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [Header("LoopGround�̎Q��"), SerializeField]
    GameObject loopGround;

    [Header("ClickScene�̎Q��"), SerializeField]
    ClickScene clickScene;

    [Header("Start�̃C���[�W"), SerializeField]
    GameObject startImage;

    [Header("���[�v������n��"), SerializeField]
    LoopGround PrologueManager;

    [Header("�^�C�g���̉摜"), SerializeField]
    Image titleImage;

    [Header("�N���A�A�C�R��"), SerializeField]
    GameObject clearIcon;

    //[Header("Audio�̎Q��"), SerializeField]
    //AudioSource audio;

    [Header("Prologue��SE"), SerializeField]
    AudioClip prologueSE;
    [SerializeField] GameObject player;
    public void Start_Button()
    {
        //clearIcon���\��
        clearIcon.SetActive(false);

        //�^�C�g����ʂ��\��
        titleImage.enabled = false;

        //�X�^�[�g�̃C���[�W���\��
        startImage.SetActive(false);

        //�v���C���[�\��
        player.SetActive(true);

        //�n�ʕ\��
        loopGround.SetActive(true);

        //�n�ʂ̃��[�v�J�n
        PrologueManager.enabled = true;

        //�J�E���g�_�E���J�n�I����N���b�N�o����X�N���v�g���A�N�e�B�u
        clickScene.enabled = true;

        //ProlougueManager��BGM�̐؂�ւ��čĐ�
        //audio.clip = prologueSE;
        //audio.Play();
    }
}