using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class DoubaoTextToSpeech : TTS
{
    [System.Serializable]
    public class AppData
    {
        public string appid;
        public string token;
        public string cluster;
    }

    [System.Serializable]
    public class UserData
    {
        public string uid;
    }

    [System.Serializable]
    public class AudioData
    {
        public string voice_type;
        public string encoding;
        public string emotion;
        public string language;
    }

    [System.Serializable]
    public class RequestData
    {
        public string reqid;
        public string text;
        public string operation;
    }

    [System.Serializable]
    public class RequestPayload
    {
        public AppData app;
        public UserData user;
        public AudioData audio;
        public RequestData request;
    }

    [System.Serializable]
    public class ResponseData
    {
        public int code;
        public string data;
    }

    private string url = "https://openspeech.bytedance.com/api/v1/tts";
    private string authorizationToken = "Bearer;mfLTvDPmPqBOJSEB9d1TJgaslfINZRDV";
    private string appId = "4724678019";
    private string token = "mfLTvDPmPqBOJSEB9d1TJgaslfINZRDV";
    private string cluster = "volcano_tts";
    private string uid = "uid123";
    private string voiceType = "zh_female_shuangkuaisisi_moon_bigtts";
    private string encoding = "mp3";
    private string emotion = "happy";
    private string language = "cn";
    private string reqId = "thisisanid";
    private string text = "字节跳动语音合成";
    private string operation = "query";

    private AudioSource audioSource;

    /// <summary>
    /// 语音合成，返回合成文本
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public override void Speak(string _msg, Action<AudioClip, string> _callback)
    {
        StartCoroutine(GetSpeech(_msg, _callback));
    }

    IEnumerator GetSpeech(string _text, Action<AudioClip, string> _callback)
    {
        StartCoroutine(SendRequest(_text, _callback));
        yield return null;
    }
    
    IEnumerator SendRequest(String message, Action<AudioClip, string> _callback)
    {
        RequestPayload payload = new RequestPayload
        {
            app = new AppData { appid = appId, token = token, cluster = cluster },
            user = new UserData { uid = uid },
            audio = new AudioData { voice_type = voiceType, encoding = encoding, emotion = emotion, language = language },
            request = new RequestData { reqid = reqId, text = message, operation = operation }
        };

        string jsonData = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", authorizationToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode);

            ResponseData responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

            if (responseData.code == 3000)
            {
                byte[] dataBytes = System.Convert.FromBase64String(responseData.data); // mp3 file
                
                
                var audioClip = (AudioClip.Create("tempAudio", dataBytes.Length, 1, 44100, false));
                // Decode mp3 file
                StartCoroutine((PlayAudio(dataBytes)));
                _callback(audioClip, message);
            }
            else
            {
                Debug.LogError("Error in response: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator PlayAudio(byte[] audioData)
    {
        string tempPath = System.IO.Path.Combine(Application.persistentDataPath, "tempAudio.mp3");
        System.IO.File.WriteAllBytes(tempPath, audioData);
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + tempPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
