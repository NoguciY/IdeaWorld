using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopGround : MonoBehaviour
{
    [SerializeField, Header("地面１")] GameObject ground1;
    [SerializeField, Header("地面 2")] GameObject ground2;
    [SerializeField, Header("地面 2")] GameObject ground3;

    [SerializeField, Header("地面が動くスピード")] float speed;

    void Start()
    {

    }

    void Update()
    {
        ground1.transform.position += Vector3.right * speed * Time.deltaTime;
        ground2.transform.position += Vector3.right * speed * Time.deltaTime;
        ground3.transform.position += Vector3.right * speed * Time.deltaTime;


        if (speed > 0)
        {



            if (ground1.transform.position.x >= 71.39999f)
            {
                ground1.transform.position = new Vector3(-52.79998f, ground1.transform.position.y, 0f);
            }
            if (ground2.transform.position.x >= 71.39999f)
            {
                ground2.transform.position = new Vector3(-52.79998f, ground2.transform.position.y, 0f);
            }
            if (ground3.transform.position.x >= 71.39999f)
            {
                ground3.transform.position = new Vector3(-52.79998f, ground3.transform.position.y, 0f);
            }

        }
        else if (speed < 0)
        {

            if (ground1.transform.position.x <= -71.39999f)
            {
                ground1.transform.position = new Vector3(52.79998f, ground1.transform.position.y, 0f);
            }
            if (ground2.transform.position.x <= -71.39999f)
            {
                ground2.transform.position = new Vector3(52.79998f, ground2.transform.position.y, 0f);
            }
            if (ground3.transform.position.x <= -71.39999f)
            {
                ground3.transform.position = new Vector3(52.79998f, ground3.transform.position.y, 0f);
            }

        }

    }
}
