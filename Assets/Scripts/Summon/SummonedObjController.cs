using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//召喚物の大体のパラメーターや
//召喚物で同じ関数をまとめるところ

public class SummonedObjController : MonoBehaviour
{
    [System.NonSerialized] public Vector3 pos;          //召喚物の位置
    [System.NonSerialized] public float lifeTime;       //召喚物の生存時間
    public float lifeSpan = 10.0f;                      //召喚物の寿命
    public float speed;                                 //移動速度

    // Update is called once per frame
    protected virtual void Update()
    {
        lifeTime += Time.deltaTime;
    }

    //召喚物の能力を記述する関数
    protected virtual void SummonedObjSkill()
    {

    }

    //召喚物を消滅させる関数
    protected virtual void DestroySummonedObj()
    {

    }
}
