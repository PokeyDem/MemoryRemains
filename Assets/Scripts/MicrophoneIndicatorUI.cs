using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MicrophoneIndicatorUI : MonoBehaviour
{
    [SerializeField] private VoiceProcessor voiceProcessor;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private TextMeshProUGUI micDataText;
    [SerializeField] private SpriteRenderer micIndicator;
    [SerializeField] private float threshold = 0.02f;
    
    [SerializeField] private AudioSource audioSource;
    private float[] _samples = new float[256];

    private void Start()
    {
        micDataText.text = Microphone.devices[0];
    }

    private void Update()
    {
        audioSource.GetOutputData(_samples, 0);

        float sum = 0f;

        for (int i = 0; i < _samples.Length; i++)
        {
            sum += _samples[i] * _samples[i];
        }
        
        float rms = Mathf.Sqrt(sum / _samples.Length);

        if (rms > threshold)
        {
            micIndicator.color = activeColor;
        }
        else
        {
            micIndicator.color = inactiveColor;
        }
    }
}
