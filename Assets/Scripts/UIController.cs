using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    public void OnAddButtonClick()
    {
        string text = uiText.text; //获取文本
        int num = Int32.Parse(text);
        num += 1;
        uiText.text = num + "";
        Debug.Log(uiText.text);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit: " + hit.transform.name);
        }
    }
   
    public void OnDecreadBUttonClick()
    {
        string text = uiText.text;
        int num = Int32.Parse(text);
        num -= 1;
        uiText.text = num + "";
    }
}
