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

    private void Start()
    {
        if (!independentLaunch)
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
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        float rms = Mathf.Sqrt(sum / samples.Length);

        if (rms < noiseThreshold) return; 

      
        int zeroCrossings = 0;
        bool isPositive = samples[0] > 0;
        float crossingThreshold = 0.01f;

        for (int i = 1; i < samples.Length; i++)
        {
            if (isPositive && samples[i] < -crossingThreshold)
            {
                zeroCrossings++;
                isPositive = false;
            }
            else if (!isPositive && samples[i] > crossingThreshold)
            {
                zeroCrossings++;
                isPositive = true;
            }
        }
        
        float pitchValue = (zeroCrossings / 2f) * (_sampleRate / samples.Length);
        
        _currentRawPitch = pitchValue;
        _currentTargetValue = Mathf.InverseLerp(minFreq, maxFreq, pitchValue);
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
        GameManager.Instance.SetMinFreq(_currentRawPitch);
    }
    
    public void SetMaxFreq()
    {
        GameManager.Instance.SetMaxFreq(_currentRawPitch);
    }
}