using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UIElements;
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent playerAgent;
    public float speed = 8.0f;
    public float Rotatespeed = 60.0f;
    public Vector3 toPosition;
    public Int32 moveFlag = 0;
    void Start()
    {
        toPosition = this.transform.position;
        playerAgent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if (moveFlag != 0)
        {
            playerAgent.SetDestination(toPosition);
            moveFlag = 0;
        }


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            bool isColliede = Physics.Raycast(ray, out hit);
            if (isColliede)
            {
                playerAgent.SetDestination(hit.point);
            }
            Debug.Log(hit.point);
        }
        // KeyControl();
    }
    private void KeyControl()
    {
        //Translate是transform组件中自带的修改自身position属性的函数，在原有的基础上进行加处理
        //Time.deltaTime ：记录每帧的用时
        //distance(一帧 移动距离)=speed（速度） * Time.deltaTime（一帧时间）

        //Translate(x,y,z,type):加的x,y,z值，由于一般设定z轴为前方，所以加z即可，Space.Self表示使用自身坐标系，这个下一章教材会讲解


        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(0, 0, speed * Time.deltaTime, Space.Self); //W 前进
            Debug.Log(speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0, 0, -speed * Time.deltaTime, Space.Self);//S 后退
        }

        //Rotate是transform组件中自带的修改自身属性的函数，在原有的基础上进行加处理
        //逆时针为减法，顺时针加法
        //y轴是top，旋转一般是人头往左或往右，所以一般我们默认以y轴为旋转轴
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(0, -Rotatespeed * Time.deltaTime, 0, Space.Self);//A逆时针旋转
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(0, Rotatespeed * Time.deltaTime, 0, Space.Self);//A逆时针旋转
        }
    }
}