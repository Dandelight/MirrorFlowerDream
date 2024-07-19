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
        //Translate��transform������Դ����޸�����position���Եĺ�������ԭ�еĻ����Ͻ��мӴ���
        //Time.deltaTime ����¼ÿ֡����ʱ
        //distance(һ֡ �ƶ�����)=speed���ٶȣ� * Time.deltaTime��һ֡ʱ�䣩

        //Translate(x,y,z,type):�ӵ�x,y,zֵ������һ���趨z��Ϊǰ�������Լ�z���ɣ�Space.Self��ʾʹ����������ϵ�������һ�½̲Ļὲ��


        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(0, 0, speed * Time.deltaTime, Space.Self); //W ǰ��
            Debug.Log(speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0, 0, -speed * Time.deltaTime, Space.Self);//S ����
        }

        //Rotate��transform������Դ����޸��������Եĺ�������ԭ�еĻ����Ͻ��мӴ���
        //��ʱ��Ϊ������˳ʱ��ӷ�
        //y����top����תһ������ͷ��������ң�����һ������Ĭ����y��Ϊ��ת��
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(0, -Rotatespeed * Time.deltaTime, 0, Space.Self);//A��ʱ����ת
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(0, Rotatespeed * Time.deltaTime, 0, Space.Self);//A��ʱ����ת
        }
    }
}