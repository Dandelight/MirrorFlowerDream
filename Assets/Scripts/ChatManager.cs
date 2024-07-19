using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
/// <summary>
/// 麦克风实时聊天
/// </summary>
public class RTSpeechHandler : MonoBehaviour
{
    /// <summary>
    /// 麦克风名称
    /// </summary>
    public string m_MicrophoneName = null;
    /// <summary>
    /// 音量大于这个值，就开始录制
    /// </summary>
    public float m_SilenceThreshold = 0.01f;
    /// <summary>
    /// 沉默限制时长
    /// </summary>
    [Header("设置几秒没声音，就停止录制")]
    public float m_RecordingTimeLimit = 2.0f;
    /// <summary>
    /// 对话状态保持时长
    /// </summary>
    [Header("设置对话状态保持时间")]
    public float m_LossAwakeTimeLimit = 10f;
    /// <summary>
    /// 锁定状态下，不记录静默时间
    /// </summary>
    [SerializeField] private bool m_LockState = false;
    /// <summary>
    /// 音频
    /// </summary>
    private AudioClip m_RecordedClip;

    /// <summary>
    /// 聊天配置
    /// </summary>
    [SerializeField] private ChatSetting m_ChatSettings;


    public Action OnAISpeakDone;

    /// <summary>   
    /// 唤醒关键词
    /// </summary>
    [SerializeField] private string m_AwakeKeyWord = string.Empty;
    /// <summary>
    /// 录制状态
    /// </summary>
    [SerializeField] private bool m_IsRecording = false;
    /// <summary>
    /// 沉默计时器
    /// </summary>
    [SerializeField] private float m_SilenceTimer = 0.0f;

    /// <summary>
    /// 播放声音
    /// </summary>
    [SerializeField] private AudioSource m_AudioSource;

    private void Awake()
    {
        OnInit();
    }

    private void OnInit()
    {
        // AI回复结束回调
        OnAISpeakDone += SpeachDoneCallBack;
    }

    private void Start()
    {

        if (m_MicrophoneName == null)
        {
            // 如果没有指定麦克风名称，则使用系统默认麦克风
            m_MicrophoneName = Microphone.devices[0];
            Debug.Log("已经开始录音");
        }
   


        // 确保麦克风准备好
        if (Microphone.IsRecording(m_MicrophoneName))
        {
            Microphone.End(m_MicrophoneName);
            Debug.Log("麦克风准备完毕");
        }

        // 启动麦克风监听
        m_RecordedClip = Microphone.Start(m_MicrophoneName, false, 30, 16000);

        while (Microphone.GetPosition(null) <= 0) { }

        // 启动录制状态检测协程
        StartCoroutine(DetectRecording());
    }

    /// <summary>
    /// 开始检测声音
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectRecording()
    {
        Debug.Log("开始");
        while (true)
        {
            float[] samples = new float[128]; // 选择合适的样本大小
            int position = Microphone.GetPosition(null);
            if (position < samples.Length)
            {
                yield return null;
                continue;
            }

            try { m_RecordedClip.GetData(samples, position - samples.Length); } catch { }

            float rms = 0.0f;
            foreach (float sample in samples)
            {
                rms += sample * sample;
            }

            rms = Mathf.Sqrt(rms / samples.Length);
            
            if (rms > m_SilenceThreshold)
            {
                m_SilenceTimer = 0.0f; // 重置静默计时器

                if (!m_IsRecording)
                {
                    StartRecording();
                }

            }
            else
            {
                if (!m_LockState)
                {
                    m_SilenceTimer += Time.deltaTime;
                }
             
                //唤醒状态，结束说话
                if (m_IsRecording && m_SilenceTimer >= m_RecordingTimeLimit)
                {
                   
                    StopRecording();
                }
            
                //沉默时间过长，结束对话状态，进入等待唤醒
                if (!m_IsRecording && m_SilenceTimer >= m_LossAwakeTimeLimit)
                {
                    PrintLog("Loss->对话连接已丢失");
                    m_IsRecording = false;
                }
            
            }

            yield return null;

        }
    }

    [SerializeField] private AudioSource m_Greeting;
    [SerializeField] private AudioClip m_GreatingVoice;

    /// <summary>
    /// 开始监听说话声音
    /// </summary>
    private void StartRecording()
    {
        m_SilenceTimer = 0.0f; // 重置静默计时器
        m_IsRecording = true;
        PrintLog("正在录制对话...");
        //停止监听，并重新开始录制，会导致唤醒的那一帧声音丢失
        Microphone.End(m_MicrophoneName);
        m_RecordedClip = Microphone.Start(m_MicrophoneName, false, 30, 16000);
    }
    /// <summary>
    /// 结束说话，发给大模型让它去识别
    /// </summary>
    private void StopRecording()
    {
        m_IsRecording = false;

        PrintLog("会话录制结束...");

        // 停止麦克风监听
        Microphone.End(m_MicrophoneName);

        // 处理音频数据
        SetRecordedAudio();
    }

    /// <summary>
    /// 开始录制监听
    /// </summary>
    public void ReStartRecord()
    {
        m_RecordedClip = Microphone.Start(m_MicrophoneName, true, 30, 16000);

        m_LockState = false;
    }


    private void SetRecordedAudio()
    {
        m_LockState = true;

        AcceptClip(m_RecordedClip);

    }


    /// <summary>
    /// 处理录制的音频数据
    /// </summary>
    /// <param name="_data"></param>
    public void AcceptClip(AudioClip _audioClip)
    {
        Debug.Log("正在进行语音识别...");
        m_ChatSettings.m_SpeechToText.SpeechToText(_audioClip, DealingTextCallback);
    }

    private void DealingTextCallback(string _msg)
    {
        Debug.Log("正在处理回调……");

        SendData(_msg);
    }

    /// <summary>
    /// 带文字发送
    /// </summary>
    /// <param name="_postWord"></param>
    public void SendData(string _postWord)
    {
        if (_postWord.Equals("")){
            Debug.Log("没有识别到内容");
            return;
}
        //添加记录聊天
        //m_ChatHistory.Add(_postWord);
        //提示词
        string _msg = _postWord;

        //发送数据
        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);
    }

    /// <summary>
    /// AI回复的信息的回调
    /// </summary>
    /// <param name="_response"></param>
    private void CallBack(string _response)
    {
        _response = _response.Trim();


        Debug.Log("收到AI回复：" + _response);

        //记录聊天
        // m_ChatHistory.Add(_response);


        m_ChatSettings.m_TextToSpeech.Speak(_response, PlayVoice);
    }

    private void PlayVoice(AudioClip _clip, string _response)
    {
        m_AudioSource.clip = _clip;
        m_AudioSource.Play();
        Debug.Log("音频时长：" + _clip.length);

        // 回复结束
        if (OnAISpeakDone != null)
        {
            OnAISpeakDone();
        }
    }


    /// <summary>
    /// 对话结束回调，启动麦克风检测
    /// </summary>
    private void SpeachDoneCallBack()
    {
        ReStartRecord();
    }

    private void PrintLog(string _log)
    {
        Debug.Log(_log);
    }

}
