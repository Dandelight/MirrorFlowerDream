using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToScene : MonoBehaviour
{
    // 要传送到的场景名称
    public string sceneName;

    //当另外一个物体与触发器碰撞的时候调用
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //切换到指定场景
            SceneManager.LoadScene(sceneName);
            Debug.Log("场景切换成功");
        }
        Debug.Log($"发生了碰撞，碰撞对象的Tag是:{other.name}");
    }
}
