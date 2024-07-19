using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToScene : MonoBehaviour
{
    // Ҫ���͵��ĳ�������
    public string sceneName;

    //������һ�������봥������ײ��ʱ�����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //�л���ָ������
            SceneManager.LoadScene(sceneName);
            Debug.Log("�����л��ɹ�");
        }
        Debug.Log($"��������ײ����ײ�����Tag��:{other.name}");
    }
}
