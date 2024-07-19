using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class SimpleInteractorControl : MonoBehaviour
{
   
    public XRRayInteractor rayInteractor;
    public XRController rightController;
    public GameObject CreateRoomObject;
    public string playName;
    //С����ƶ��ٶ�
    public float Speech = 2.0f;
    RaycastHit hit;
    private void Update()
    {
        //�洢ҡ�˵���һ��x,y���ϵ��ƶ����� 
        Vector2 result;

        //�ж�����Ƿ�۶��˰��
        bool ret;

        //out ����һ������������primary2DAxis �޸ĵ�ֵ����ͨ��out ����Ĳ���Ҳ����result���
        ret = rightController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float trigeRes);
       
        if (ret && trigeRes>0.5)
        {
            Vector3 currentPosition = this.transform.position;
            string Collername = getRayCollerName();
            if (Collername == CreateRoomObject.name)
            {
                SceneManager.LoadScene(playName);
            }
            Debug.Log(Collername);
           // this.transform.Translate(Speech * result.x * Time.deltaTime, 0, Speech * result.y * Time.deltaTime);
        }


    }
    Vector3 getRayPoint()
    {
        // ���Hover

        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            string name = hit.collider.name;
            Debug.Log(name);
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
    string getRayCollerName()
    {
        // ���Hover

        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            string name = hit.collider.name;
            Debug.Log(name);
            return name;

        }
        else
        {
            return "";
        }
    }
    Vector3 RayClick()
    {
        // ���Hover

        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            string name = hit.collider.name;
            Debug.Log(name);
            return hit.point;

        }
        else
        {
            return Vector3.zero;
        }
    }
}
