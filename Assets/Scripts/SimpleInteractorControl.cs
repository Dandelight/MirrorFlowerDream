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
    //小球的移动速度
    public float Speech = 2.0f;
    RaycastHit hit;
    private void Update()
    {
        //存储摇杆的在一个x,y轴上的移动数据 
        Vector2 result;

        //判断玩家是否扣动了扳机
        bool ret;

        //out 类似一个引用声明，primary2DAxis 修改的值可以通过out 后面的参数也就是result获得
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
        // 鼠标Hover

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
        // 鼠标Hover

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
        // 鼠标Hover

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
