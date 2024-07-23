using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [Header("LoopGroundの参照"), SerializeField]
    GameObject loopGround;

    [Header("ClickSceneの参照"), SerializeField]
    ClickScene clickScene;

    [Header("Startのイメージ"), SerializeField]
    GameObject startImage;

    [Header("ループさせる地面"), SerializeField]
    LoopGround PrologueManager;

    [Header("タイトルの画像"), SerializeField]
    Image titleImage;

    [Header("クリアアイコン"), SerializeField]
    GameObject clearIcon;

    //[Header("Audioの参照"), SerializeField]
    //AudioSource audio;

    [Header("PrologueのSE"), SerializeField]
    AudioClip prologueSE;
    [SerializeField] GameObject player;
    public void Start_Button()
    {
        //clearIconを非表示
        clearIcon.SetActive(false);

        //タイトル画面を非表示
        titleImage.enabled = false;

        //スタートのイメージを非表示
        startImage.SetActive(false);

        //プレイヤー表示
        player.SetActive(true);

        //地面表示
        loopGround.SetActive(true);

        //地面のループ開始
        PrologueManager.enabled = true;

        //カウントダウン開始終了後クリック出来るスクリプトをアクティブ
        clickScene.enabled = true;

        //ProlougueManagerのBGMの切り替えて再生
        //audio.clip = prologueSE;
        //audio.Play();
    }
}