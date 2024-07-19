using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
/// <summary>
/// ��˷�ʵʱ����
/// </summary>
public class RTSpeechHandler : MonoBehaviour
{
    /// <summary>
    /// ��˷�����
    /// </summary>
    public string m_MicrophoneName = null;
    /// <summary>
    /// �����������ֵ���Ϳ�ʼ¼��
    /// </summary>
    public float m_SilenceThreshold = 0.01f;
    /// <summary>
    /// ��Ĭ����ʱ��
    /// </summary>
    [Header("���ü���û��������ֹͣ¼��")]
    public float m_RecordingTimeLimit = 2.0f;
    /// <summary>
    /// �Ի�״̬����ʱ��
    /// </summary>
    [Header("���öԻ�״̬����ʱ��")]
    public float m_LossAwakeTimeLimit = 10f;
    /// <summary>
    /// ����״̬�£�����¼��Ĭʱ��
    /// </summary>
    [SerializeField] private bool m_LockState = false;
    /// <summary>
    /// ��Ƶ
    /// </summary>
    private AudioClip m_RecordedClip;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] private ChatSetting m_ChatSettings;


    public Action OnAISpeakDone;

    /// <summary>   
    /// ���ѹؼ���
    /// </summary>
    [SerializeField] private string m_AwakeKeyWord = string.Empty;
    /// <summary>
    /// ¼��״̬
    /// </summary>
    [SerializeField] private bool m_IsRecording = false;
    /// <summary>
    /// ��Ĭ��ʱ��
    /// </summary>
    [SerializeField] private float m_SilenceTimer = 0.0f;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] private AudioSource m_AudioSource;

    private void Awake()
    {
        OnInit();
    }

    private void OnInit()
    {
        // AI�ظ������ص�
        OnAISpeakDone += SpeachDoneCallBack;
    }

    private void Start()
    {

        if (m_MicrophoneName == null)
        {
            // ���û��ָ����˷����ƣ���ʹ��ϵͳĬ����˷�
            m_MicrophoneName = Microphone.devices[0];
            Debug.Log("�Ѿ���ʼ¼��");
        }
   


        // ȷ����˷�׼����
        if (Microphone.IsRecording(m_MicrophoneName))
        {
            Microphone.End(m_MicrophoneName);
            Debug.Log("��˷�׼�����");
        }

        // ������˷����
        m_RecordedClip = Microphone.Start(m_MicrophoneName, false, 30, 16000);

        while (Microphone.GetPosition(null) <= 0) { }

        // ����¼��״̬���Э��
        StartCoroutine(DetectRecording());
    }

    /// <summary>
    /// ��ʼ�������
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectRecording()
    {
        Debug.Log("��ʼ");
        while (true)
        {
            float[] samples = new float[128]; // ѡ����ʵ�������С
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
                m_SilenceTimer = 0.0f; // ���þ�Ĭ��ʱ��

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
             
                //����״̬������˵��
                if (m_IsRecording && m_SilenceTimer >= m_RecordingTimeLimit)
                {
                   
                    StopRecording();
                }
            
                //��Ĭʱ������������Ի�״̬������ȴ�����
                if (!m_IsRecording && m_SilenceTimer >= m_LossAwakeTimeLimit)
                {
                    PrintLog("Loss->�Ի������Ѷ�ʧ");
                    m_IsRecording = false;
                }
            
            }

            yield return null;

        }
    }

    [SerializeField] private AudioSource m_Greeting;
    [SerializeField] private AudioClip m_GreatingVoice;

    /// <summary>
    /// ��ʼ����˵������
    /// </summary>
    private void StartRecording()
    {
        m_SilenceTimer = 0.0f; // ���þ�Ĭ��ʱ��
        m_IsRecording = true;
        PrintLog("����¼�ƶԻ�...");
        //ֹͣ�����������¿�ʼ¼�ƣ��ᵼ�»��ѵ���һ֡������ʧ
        Microphone.End(m_MicrophoneName);
        m_RecordedClip = Microphone.Start(m_MicrophoneName, false, 30, 16000);
    }
    /// <summary>
    /// ����˵����������ģ������ȥʶ��
    /// </summary>
    private void StopRecording()
    {
        m_IsRecording = false;

        PrintLog("�Ự¼�ƽ���...");

        // ֹͣ��˷����
        Microphone.End(m_MicrophoneName);

        // ������Ƶ����
        SetRecordedAudio();
    }

    /// <summary>
    /// ��ʼ¼�Ƽ���
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
    /// ����¼�Ƶ���Ƶ����
    /// </summary>
    /// <param name="_data"></param>
    public void AcceptClip(AudioClip _audioClip)
    {
        Debug.Log("���ڽ�������ʶ��...");
        m_ChatSettings.m_SpeechToText.SpeechToText(_audioClip, DealingTextCallback);
    }

    private void DealingTextCallback(string _msg)
    {
        Debug.Log("���ڴ���ص�����");

        SendData(_msg);
    }

    /// <summary>
    /// �����ַ���
    /// </summary>
    /// <param name="_postWord"></param>
    public void SendData(string _postWord)
    {
        if (_postWord.Equals("")){
            Debug.Log("û��ʶ������");
            return;
}
        //��Ӽ�¼����
        //m_ChatHistory.Add(_postWord);
        //��ʾ��
        string _msg = _postWord;

        //��������
        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);
    }

    /// <summary>
    /// AI�ظ�����Ϣ�Ļص�
    /// </summary>
    /// <param name="_response"></param>
    private void CallBack(string _response)
    {
        _response = _response.Trim();


        Debug.Log("�յ�AI�ظ���" + _response);

        //��¼����
        // m_ChatHistory.Add(_response);


        m_ChatSettings.m_TextToSpeech.Speak(_response, PlayVoice);
    }

    private void PlayVoice(AudioClip _clip, string _response)
    {
        m_AudioSource.clip = _clip;
        m_AudioSource.Play();
        Debug.Log("��Ƶʱ����" + _clip.length);

        // �ظ�����
        if (OnAISpeakDone != null)
        {
            OnAISpeakDone();
        }
    }


    /// <summary>
    /// �Ի������ص���������˷���
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
