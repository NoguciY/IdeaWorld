using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkAndParentPool : MonoBehaviour
{

    [Header("親にしたいオブジェクト")]
    public Transform parent;


    [Header("Inkとして使いたいオブジェクト")]
    public GameObject inkPrefab;


    [Header("[ 事前に作っておくオブジェクトの数 ]")]
    //親の生成数
    [Header("フィールドに描ける最大数")]
    public int parentProduction;

    //Inkの生成数
    [Header("Inkの量")]
    public int inksProduced;


    //[ 親とInkを格納している配列 ]=====================================================

    //親
    public Queue<Transform> parentQueue;
    //Ink
    public Queue<GameObject> inkQueue;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            //Debug.Log("pool  " + parentQueue.Count);
        }
    }

    void Start()
    {
        //Queueのインスタンス生成
        parentQueue = new Queue<Transform>();
        inkQueue = new Queue<GameObject>();

        //QueueにPrefabを生成して登録
        for (int i = 0; i < parentProduction; i++)
        {
            //parent生成
            Transform p=Instantiate(parent, transform.position, Quaternion.identity, transform);


            //回収処理とかを呼び出してもらうためにpoolの参照を渡しておく
            p.GetComponent<Ink>().pool = this;

            parentQueue.Enqueue(p);
        }
        for (int i = 0; i < inksProduced; i++)
        {
            inkQueue.Enqueue(Instantiate(inkPrefab, transform));
        }
    }
    //Inkの貸出
    public GameObject LendInk(Transform parent,Vector3 position)
    {
        if (inkQueue.Count <= 0||parent==null)
        {
            Debug.Log("Ink貸せないよ　　(´・ω・`)");
            return null;
        }

        GameObject ink = inkQueue.Dequeue();
        ink.transform.position = position;
        ink.transform.SetParent(parent);
        ink.gameObject.SetActive(true);
        return ink;
    }
    //Parentの貸出
    public Transform LendParent(Vector3 position)
    {
        if (parentQueue.Count <= 0)
        {
            Debug.Log("parentがないよ　　(´・ω・`)");
            return null;
        }
        Transform parent = parentQueue.Dequeue();
        parent.transform.position = position;
        parent.gameObject.SetActive(true);

        return parent;
    }
    //Ink回収
    public void CollectInk(GameObject ink)
    {
        ink.gameObject.SetActive(false);
        inkQueue.Enqueue(ink);
        ink.transform.parent = transform;
    }
    //Parent回収
    public void CollectParent(Transform parent)
    {
        parent.gameObject.SetActive(false);
        parentQueue.Enqueue(parent);
    }
}