using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChestSound : MonoBehaviour
{
    [Header("•ó” ŠJ‚¢‚½Žž‚Ì‰¹"), SerializeField] AudioClip openSound;
    [Header("–œ”\–ò‚ðŽè‚É“ü‚ê‚½Žž‚Ì‰¹"), SerializeField] AudioClip getSound;

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