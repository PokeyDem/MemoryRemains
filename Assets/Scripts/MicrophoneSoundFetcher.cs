using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneSoundFetcher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    private AudioSource _audioSource;
    private string _activeDevice;
    private float _freq;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        StartMicrophone();
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(_activeDevice) && Microphone.IsRecording(_activeDevice))
        {
            statusText.text = $"Device: {_activeDevice} | Is Active | Freq: {_freq}";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = "No Microphone Found";
            statusText.color = Color.red;
        }
    }

    private void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            _activeDevice = Microphone.devices[0];
            _audioSource.clip = Microphone.Start(_activeDevice, true, 1,AudioSettings.outputSampleRate);
            _audioSource.loop = true;
            
            while (!(Microphone.GetPosition(null) > 0)) { }
            
            _audioSource.Play();
        }
    }

    public void SetFrequency(float freq)
    {
        _freq = freq;
    }
}
