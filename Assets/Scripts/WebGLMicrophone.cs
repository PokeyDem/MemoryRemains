using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLMicrophone : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void InitMicrophoneJS();

    [DllImport("__Internal")]
    private static extern int GetMicrophoneDataJS(float[] data, int length);

    private float[] _micData = new float[1024];
    public bool isInitialized { get; private set; }

    #if UNITY_EDITOR || !UNITY_WEBGL
    private AudioClip _micClip;
    private string _deviceName;
    #endif
    
    public void RequestMicrophone()
    {
        if (isInitialized) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            InitMicrophoneJS();
            isInitialized = true;
        #else
        if (Microphone.devices.Length > 0)
        {
            _deviceName = Microphone.devices[0];
          
            _micClip = Microphone.Start(_deviceName, true, 1, 44100);
            isInitialized = true;
            Debug.Log($"Микрофон запущен в редакторе: {_deviceName}");
        }
        else
        {
            Debug.LogError("В редакторе не найдено ни одного микрофона!");
        }
        #endif
    }

  
    public float[] GetAudioData()
    {
        if (!isInitialized) return _micData;

        #if UNITY_WEBGL && !UNITY_EDITOR
            GetMicrophoneDataJS(_micData, _micData.Length);
        #else
    
        int micPosition = Microphone.GetPosition(_deviceName) - _micData.Length + 1;
        
        if (micPosition < 0) return _micData; 
        
        _micClip.GetData(_micData, micPosition);
        #endif

        return _micData;
    }
}
