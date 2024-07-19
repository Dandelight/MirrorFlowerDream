using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XunfeiSettings : MonoBehaviour
{
    #region 参数
    /// <summary>
    /// 讯飞的AppID
    /// </summary>
    [Header("填写app id")]
    [SerializeField] public string m_AppID = "appid";
    /// <summary>
    /// 讯飞的APIKey
    /// </summary>
    [Header("填写api key")]
    [SerializeField] public string m_APIKey = "apikey";
    /// <summary>
    /// 讯飞的APISecret
    /// </summary>
    [Header("填写secret key")]
    [SerializeField] public string m_APISecret = "apisecret";

    #endregion
}
