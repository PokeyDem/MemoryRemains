using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceProcessor : MonoBehaviour
{
    [Header("Pitch detection settings")] 
    
    [SerializeField] private float minFreq = 100f;
    [SerializeField] private float maxFreq = 800f;
    [SerializeField] private float noiseThreshold = 0.005f;
    [SerializeField] private float lerpSpeed = 10f;
    
    [SerializeField] private MicrophoneSoundFetcher microphoneSoundFetcher;
    [SerializeField] private UIVisualsController uiVisualsController;

    [Header("Temp")] 
    [SerializeField] private TextMeshProUGUI minFreqField;
    [SerializeField] private TextMeshProUGUI maxFreqField;

    private AudioSource _audioSource;
    private float[] _spectrumData = new float[1024];
    private float _currentTargetValue = 0f;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        if (Microphone.IsRecording(null))
        {
            AnalyzeSound();
        }
    }

    private void AnalyzeSound()
    {
        _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.BlackmanHarris);

        float maxVolume = 0f;
        int maxIndex = 0;

        for (int i = 0; i < _spectrumData.Length; i++)
        {
            if (_spectrumData[i] > maxVolume && _spectrumData[i] > noiseThreshold)
            {
                maxVolume = _spectrumData[i];
                maxIndex = i;
            }
        }

        if (maxVolume > noiseThreshold)
        {
            float freqIndex = maxIndex;

            if (maxIndex > 0 && maxIndex < _spectrumData.Length - 1)
            {
                float dL = _spectrumData[maxIndex - 1] / _spectrumData[maxIndex];
                float dR = _spectrumData[maxIndex + 1] / _spectrumData[maxIndex];
                freqIndex += 0.5f * (dR * dR - dL * dL);
            }

            float pitchValue = freqIndex * (AudioSettings.outputSampleRate / 2f) / _spectrumData.Length;

            _currentTargetValue = Mathf.InverseLerp(minFreq, maxFreq, pitchValue);
            
            microphoneSoundFetcher.SetFrequency(_currentTargetValue);
            uiVisualsController.SetSliderValue(_currentTargetValue);
            
            //process UI
        }
    }

    public float GetTargetValue()
    {
        return _currentTargetValue;
    }

    public void SetMinFreq()
    {
        Debug.Log(minFreqField.text.Length);
        int newMinFreq = int.Parse(minFreqField.text.Replace("\u200B", ""));
        if (newMinFreq > 0)
            minFreq = newMinFreq;
    }
    
    public void SetMaxFreq()
    {
        int newMaxFreq = int.Parse(maxFreqField.text.Replace("\u200B", ""));
        if (newMaxFreq > 0)
            minFreq = newMaxFreq;
    }
}
