using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerContoler : MonoBehaviour
{
    // �����ҿ����������� 
    public XRController rightController;
    //С����ƶ��ٶ�
    public float Speech = 2.0f;
    void Start()
    {

    }

    void Update()
    {
        //�洢ҡ�˵���һ��x,y���ϵ��ƶ����� 
        Vector2 result;


        //�ж�����Ƿ��ƶ���ҡ��
        bool ret;

        //out ����һ������������primary2DAxis �޸ĵ�ֵ����ͨ��out ����Ĳ���Ҳ����result���
        ret = rightController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out result);

        if (ret)
        {
            Vector3 currentPosition = this.transform.position;
            this.transform.Translate(Speech * result.x * Time.deltaTime, 0, Speech * result.y * Time.deltaTime);

        }

    }
}
