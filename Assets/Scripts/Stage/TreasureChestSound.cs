using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChestSound : MonoBehaviour
{
    [Header("宝箱開いた時の音"), SerializeField] AudioClip openSound;
    [Header("万能薬を手に入れた時の音"), SerializeField] AudioClip getSound;

    [SerializeField] AudioSource audioSource;
    public void ChestOpen()
    {
        audioSource.PlayOneShot(openSound);
    }
    public void GetSound()
    {
        audioSource.PlayOneShot(getSound);

    }
}