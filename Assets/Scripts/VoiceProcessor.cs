using UnityEngine;

public class VoiceProcessor : MonoBehaviour
{
    [Header("Pitch detection settings")] 
    [SerializeField] private float minFreq = 80f;
    [SerializeField] private float maxFreq = 500f;
    [SerializeField] private float noiseThreshold = 0.005f;
    
    [SerializeField] private UIVisualsController uiVisualsController;
    [SerializeField] private bool independentLaunch;

    private float _currentTargetValue = 0f;
    private float _currentRawPitch;
    private float _sampleRate = 44100f; 
    
    private const float AbsoluteMinVoice = 60f;
    private const float AbsoluteMaxVoice = 1200f;

    private void Start()
    {
        if (!independentLaunch && GameManager.Instance != null)
        {
            minFreq = GameManager.Instance.GetMinFreq();
            maxFreq = GameManager.Instance.GetMaxFreq();
        }
    }

    private void Update()
    {
        if (WebGLMicrophone.Instance != null && WebGLMicrophone.Instance.isInitialized)
        {
            AnalyzeSound();
        }
    }

    private void AnalyzeSound()
    {
        float[] samples = WebGLMicrophone.Instance.GetAudioData();
        if (samples == null || samples.Length == 0) return;

        float sum = 0f;
        float maxAmplitude = 0f;
        
        for (int i = 0; i < samples.Length; i++)
        {
            float absVal = Mathf.Abs(samples[i]);
            sum += samples[i] * samples[i];
            if (absVal > maxAmplitude) maxAmplitude = absVal;
        }
        
        Debug.Log($"Max amplitude: {maxAmplitude}");
        
        float rms = Mathf.Sqrt(sum / samples.Length);
        
        Debug.Log($"RMS: {rms}");
        
        if (rms < noiseThreshold || maxAmplitude < 0.001f) return;

        float[] normalizedSamples = new float[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            normalizedSamples[i] = samples[i] / maxAmplitude;
        }
        
        int minLag = Mathf.FloorToInt(_sampleRate / AbsoluteMaxVoice);
        int maxLag = Mathf.CeilToInt(_sampleRate / AbsoluteMinVoice);
        
        if (maxLag >= normalizedSamples.Length) 
        {
            maxLag = normalizedSamples.Length - 1;
        }
        if (minLag < 1) minLag = 1;

        float[] correlations = new float[maxLag + 1];

        for (int lag = minLag; lag <= maxLag; lag++)
        {
            float correlation = 0f;
            int activeSamples = normalizedSamples.Length - lag;
            
            for (int i = 0; i < activeSamples; i++)
            {
                correlation += normalizedSamples[i] * normalizedSamples[i + lag];
            }
            correlations[lag] = correlation / activeSamples; 
        }

        int bestLag = -1;
        float confidenceThreshold = 0.15f; 
        float fallbackMax = 0f;
        int fallbackLag = -1;

        for (int lag = minLag + 1; lag < maxLag; lag++)
        {
            if (correlations[lag] > fallbackMax)
            {
                fallbackMax = correlations[lag];
                fallbackLag = lag;
            }

            if (correlations[lag] > correlations[lag - 1] && correlations[lag] > correlations[lag + 1])
            {
                if (correlations[lag] > confidenceThreshold)
                {
                    bestLag = lag;
                    break;
                }
            }
        }

        if (bestLag == -1 && fallbackLag != -1)
        {
            bestLag = fallbackLag;
        }

        if (bestLag > 0)
        {
            float pitchValue = _sampleRate / bestLag;
            _currentRawPitch = pitchValue;
            
            float logMin = Mathf.Log(minFreq, 2f);
            float logMax = Mathf.Log(maxFreq, 2f);
            float logPitch = Mathf.Log(pitchValue, 2f);
            
            _currentTargetValue = Mathf.InverseLerp(logMin, logMax, logPitch);
        }
    }

    public float GetTargetValue()
    {
        return _currentTargetValue;
    }

    public float GetRawPitch()
    {
        return _currentRawPitch;
    }

    public void SetMinFreq()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetMinFreq(_currentRawPitch);
    }
    
    public void SetMaxFreq()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetMaxFreq(_currentRawPitch);
    }
}