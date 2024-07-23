using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChestSound : MonoBehaviour
{
    [Header("�󔠊J�������̉�"), SerializeField] AudioClip openSound;
    [Header("���\�����ɓ��ꂽ���̉�"), SerializeField] AudioClip getSound;

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