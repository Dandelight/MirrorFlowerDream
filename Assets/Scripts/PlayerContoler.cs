using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerContoler : MonoBehaviour
{
    // 传入右控制器的引用 
    public XRController rightController;
    //小球的移动速度
    public float Speech = 2.0f;
    void Start()
    {

    }

    void Update()
    {
        //存储摇杆的在一个x,y轴上的移动数据 
        Vector2 result;


        //判断玩家是否移动了摇杆
        bool ret;

        //out 类似一个引用声明，primary2DAxis 修改的值可以通过out 后面的参数也就是result获得
        ret = rightController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out result);

        if (ret)
        {
            Vector3 currentPosition = this.transform.position;
            this.transform.Translate(Speech * result.x * Time.deltaTime, 0, Speech * result.y * Time.deltaTime);

        }

    }
}
