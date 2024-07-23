using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField,Header("ドアが開く速度")] float openSpeed;

    [SerializeField, Header("ドアがどれぐらい上がるかの高さ")] float openHeight;

    float height;

    private void Start()
    {
        height = transform.position.y + openHeight;
    }
    public void DoorOpen()
    {
        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        while (transform.position.y <= height)
        {
            transform.localPosition +=Vector3.up * openSpeed * Time.deltaTime;
            yield return null;

        }
    }
}
